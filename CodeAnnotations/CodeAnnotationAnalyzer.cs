// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnnotationAnalyzer.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AttributeAnalyzer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CodeAnnotationPack.CodeAnnotations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CodeAnnotationPack.Options;
  using JetBrains.Annotations;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
  using JetBrains.ReSharper.Feature.Services.Daemon;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Resources.Shell;

	/// <summary>
  /// Defines the AttributeAnalyzer class.
  /// </summary>
  [ElementProblemAnalyzer(new[]
  {
    typeof(ITypeMemberDeclaration)
  })]
  public class CodeAnnotationAnalyzer : ElementProblemAnalyzer<ITypeMemberDeclaration>
  {
    #region Fields

    /// <summary>
    /// Gets or sets the last run timestamp.
    /// </summary>
    /// <value>The last run timestamp.</value>
    private long lastRunTimestamp;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the alternative assertion.
    /// </summary>
    /// <value>The alternative assertion.</value>
    [NotNull]
    public IEnumerable<string> AlternativeAssertions { get; private set; }

    /// <summary>
    /// Gets or sets the non public assertion code.
    /// </summary>
    /// <value>The non public assertion code.</value>
    [NotNull]
    public string NonPublicAssertionCode { get; set; }

    /// <summary>
    /// Gets or sets the public assertion code.
    /// </summary>
    /// <value>The public assertion code.</value>
    [NotNull]
    public string PublicAssertionCode { get; set; }

    #endregion

    #region Methods

    /// <summary>Runs the specified element.</summary>
    /// <param name="treeNode">The tree node.</param>
    /// <param name="data">The data.</param>
    /// <param name="consumer">The consumer.</param>
    protected override void Run(ITypeMemberDeclaration treeNode, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
      CodeAnnotationModel model = null;

      var daemon = (DaemonProcessBase)data.Process;
      if (daemon.LastRunTimestamp != this.lastRunTimestamp)
      {
        this.SetAttributes(data, treeNode);
        this.lastRunTimestamp = daemon.LastRunTimestamp;
      }

      var range = DocumentRange.InvalidRange;

      var method = treeNode as IMethodDeclaration;
      if (method != null)
      {
        model = new CodeAnnotationModel(this.PublicAssertionCode, this.NonPublicAssertionCode, this.AlternativeAssertions);
        model.Inspect(method);
        range = method.NameIdentifier.GetDocumentRange();
      }

      if (model == null)
      {
        var property = treeNode as IPropertyDeclaration;
        if (property != null)
        {
          model = new CodeAnnotationModel(this.PublicAssertionCode, this.NonPublicAssertionCode, this.AlternativeAssertions);
          model.Inspect(property);
          range = property.NameIdentifier.GetDocumentRange();
        }
      }

      if (model == null)
      {
        var constructor = treeNode as IConstructorDeclaration;
        if (constructor != null)
        {
          model = new CodeAnnotationModel(this.PublicAssertionCode, this.NonPublicAssertionCode, this.AlternativeAssertions);
          model.Inspect(constructor);
          range = constructor.Name.GetDocumentRange();
        }
      }

      if (model == null)
      {
        var indexer = treeNode as IIndexerDeclaration;
        if (indexer != null)
        {
          model = new CodeAnnotationModel(this.PublicAssertionCode, this.NonPublicAssertionCode, this.AlternativeAssertions);
          model.Inspect(indexer);
          range = indexer.NameIdentifier.GetDocumentRange();
        }
      }

      if (model == null || model.IsAnnotated)
      {
        return;
      }

      var highlight = new CodeAnnotationHighlight(model, range);
      consumer.AddHighlighting(highlight, range, treeNode.GetContainingFile());
    }

    /// <summary>Pres the execute.</summary>
    /// <param name="data">The data.</param>
    /// <param name="treeNode">The tree node.</param>
    private void SetAttributes(ElementProblemAnalyzerData data, ITreeNode treeNode)
    {
      var service = Shell.Instance.GetComponent<CodeAnnotationPackService>();
      var settings = service.GetSettings();

      var publicAssertion = settings.PublicAssertion ?? string.Empty;
      var nonPublicAssertion = settings.NonPublicAssertion ?? string.Empty;
      var alternativeAssertions = settings.AlternativeAssertionsList;

      var psiModule = treeNode.GetPsiModule();
      var psiServices = treeNode.GetPsiServices();
      var context = treeNode.GetResolveContext();

      var attributesSet = psiServices.Symbols.GetModuleAttributes(psiModule, context);
      if (attributesSet == null)
      {
        return;
      }

      foreach (var instance in attributesSet.GetAttributeInstances(true))
      {
        var attributeName = instance.GetClrName().ShortName;
        if (attributeName != "ReSharperSettingAttribute")
        {
          continue;
        }

        var nameParameter = instance.PositionParameter(0);
        if (nameParameter.IsBadValue)
        {
          continue;
        }

        if (!nameParameter.IsConstant)
        {
          continue;
        }

        var name = nameParameter.ConstantValue.Value as string;
        if (name != "CodeAnnotations.PublicAssertion" && name != "CodeAnnotations.NonPublicAssertion" && name != "CodeAnnotations.AlternativeAssertions")
        {
          continue;
        }

        var valueParameter = instance.PositionParameter(1);
        if (!valueParameter.IsConstant)
        {
          continue;
        }

        var value = valueParameter.ConstantValue.Value as string ?? string.Empty;
        if (name == "CodeAnnotations.AlternativeAssertions")
        {
          if (!string.IsNullOrEmpty(value))
          {
            alternativeAssertions = new List<string>(value.Split(CodeAnnotationPackSettings.AlternativeAssertionsChars, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()));
          }
          else
          {
            alternativeAssertions = Enumerable.Empty<string>();
          }
        }
        else if (name == "CodeAnnotations.PublicAssertion")
        {
          publicAssertion = value;
        }
        else
        {
          nonPublicAssertion = value;
        }
      }

      if (publicAssertion == null)
      {
        publicAssertion = "#";
      }

      if (nonPublicAssertion == null)
      {
        nonPublicAssertion = "#";
      }

      this.PublicAssertionCode = publicAssertion == "#" ? string.Empty : publicAssertion;
      this.NonPublicAssertionCode = nonPublicAssertion == "#" ? string.Empty : nonPublicAssertion;
      this.AlternativeAssertions = alternativeAssertions;
    }

    #endregion
  }
}
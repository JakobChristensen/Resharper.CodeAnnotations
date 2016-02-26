// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnnotationFix.cs" company="">
//   
// </copyright>
// <summary>
//   The convert path to id fix.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CodeAnnotationPack.CodeAnnotations
{
  using System;
  using JetBrains.Annotations;
  using JetBrains.Application.Progress;
  using JetBrains.DocumentManagers;
  using JetBrains.DocumentModel;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Feature.Services.QuickFixes;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp.Parsing;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Text;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// The convert path to id fix.
  /// </summary>
  [QuickFix]
  public sealed class CodeAnnotationFix : QuickFixBase
  {
    #region Constructors and Destructors

    /// <summary>Initializes a new instance of the <see cref="CodeAnnotationFix"/> class.</summary>
    /// <param name="highlight">The highlight.</param>
    public CodeAnnotationFix([NotNull] CodeAnnotationHighlight highlight)
    {
      this.Highlight = highlight;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Popup menu item text
    /// </summary>
    public override string Text
    {
      get
      {
        return "Add NotNull or CanBeNull attributes [Code Annotation Pack]";
      }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the highlight.
    /// </summary>
    /// <value>The highlight.</value>
    [NotNull]
    private CodeAnnotationHighlight Highlight { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>Adds the blank line after.</summary>
    /// <param name="statement">The statement.</param>
    public void AddBlankLineAfter([NotNull] IStatement statement)
    {
      if (statement == null)
      {
        throw new ArgumentNullException("statement");
      }

      var newLineText = new StringBuffer("\r\n");
      ITreeNode newLine = TreeElementFactory.CreateLeafElement(CSharpTokenType.NEW_LINE, newLineText, 0, newLineText.Length);

      ModificationUtil.AddChildAfter(statement, newLine);
    }

    /// <summary>Check if this action is available at the constructed context.
    ///             Actions could store precalculated info in <paramref name="cache"/> to share it between different actions</summary>
    /// <param name="cache">The cache.</param>
    /// <returns>true if this bulb action is available, false otherwise.</returns>
    public override bool IsAvailable(IUserDataHolder cache)
    {
      return true;
    }

    #endregion

    #region Methods

    /// <summary>Executes QuickFix or ContextAction. Returns post-execute method.</summary>
    /// <param name="solution">The solution.</param>
    /// <param name="progress">The progress.</param>
    /// <returns>Action to execute after document and PSI transaction finish. Use to open TextControls, navigate caret, etc.</returns>
    protected override Action<ITextControl> ExecutePsiTransaction([NotNull] ISolution solution, [NotNull] IProgressIndicator progress)
    {
      this.FixAnnotations();
      this.FixReturn();
      this.RemoveAssertions();
      this.FixAssertions();

      if (this.Highlight.Model.Body != null)
      {
        this.FormatBody(this.Highlight.Model.Body);
      }

      return null;
    }

    /// <summary>Fixes the parameters.</summary>
    private void FixAnnotations()
    {
      foreach (var parameterDescriptor in this.Highlight.Model.Parameters)
      {
        var parameter = parameterDescriptor.Parameter;
        if (parameter == null)
        {
          continue;
        }

        if (parameterDescriptor.ExpectedAttribute != parameterDescriptor.AppliedAttribute)
        {
          parameter.Annotate(parameterDescriptor.ExpectedAttribute);
        }
      }
    }

    /// <summary>Inserts the assertions.</summary>
    private void FixAssertions()
    {
      var body = this.Highlight.Model.Body;
      if (body == null)
      {
        return;
      }

      IStatement last = null;
      var anchor = body.Statements.FirstOrDefault();

      var factory = CSharpElementFactory.GetInstance(body.GetPsiModule());

      foreach (var parameterDescriptor in this.Highlight.Model.Parameters)
      {
        var code = parameterDescriptor.Code;
        if (string.IsNullOrEmpty(code))
        {
          continue;
        }

        var statement = factory.CreateStatement(code);
        body.AddStatementBefore(statement, anchor);

        if (last == null)
        {
          last = statement;
        }
      }

      if (last != null)
      {
        this.AddBlankLineAfter(last);
      }
    }

    /// <summary>Fixes the return.</summary>
    private void FixReturn()
    {
      if (this.Highlight.Model.ExpectedReturn != this.Highlight.Model.AppliedReturn)
      {
        this.Highlight.Model.TypeMember.Annotate(this.Highlight.Model.ExpectedReturn);
      }
    }

    /// <summary>The format body.</summary>
    /// <param name="body">The body.</param>
    private void FormatBody(IBlock body)
    {
      if (!body.IsPhysical())
      {
        return;
      }

      var documentRange = body.GetDocumentRange();
      if (!documentRange.IsValid())
      {
        return;
      }

      var containingFile = body.GetContainingFile();
      if (containingFile == null)
      {
        return;
      }

      var psiSourceFile = containingFile.GetSourceFile();
      if (psiSourceFile == null)
      {
        return;
      }

      var document = psiSourceFile.Document;
      var solution = body.GetPsiServices().Solution;

      var languageService = body.Language.LanguageService();
      if (languageService == null)
      {
        return;
      }

      var codeFormatter = languageService.CodeFormatter;
      if (codeFormatter == null)
      {
        return;
      }

      var rangeMarker = new DocumentRange(document, documentRange.TextRange).CreateRangeMarker(DocumentManager.GetInstance(solution));

      containingFile.OptimizeImportsAndRefs(rangeMarker, false, true, NullProgressIndicator.Instance);
      codeFormatter.Format(body, CodeFormatProfile.DEFAULT);
    }

    /// <summary>Removes the assertions.</summary>
    private void RemoveAssertions()
    {
      var body = this.Highlight.Model.Body;
      if (body == null)
      {
        return;
      }

      foreach (var parameterDescriptor in this.Highlight.Model.Parameters)
      {
        if (parameterDescriptor.Assertion != null)
        {
          body.RemoveStatement((ICSharpStatement)parameterDescriptor.Assertion);
        }
      }
    }

    #endregion
  }
}
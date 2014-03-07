// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnnotation.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the value abnalysis attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ValueAnalysisPack.CodeAnnotations
{
  using System;
  using JetBrains.Annotations;
  using JetBrains.Application;
  using JetBrains.Application.Progress;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeAnnotations;
  using JetBrains.ReSharper.Psi.ControlFlow.CSharp;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Resolve;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;
  using JetBrains.UI.Application.Progress;
  using JetBrains.Util;

  /// <summary>
  /// Defines the value abnalysis attribute.
  /// </summary>
  public enum CodeAnnotationAttribute
  {
    /// <summary>
    /// The undefined field.
    /// </summary>
    Undefined, 

    /// <summary>
    /// The undefined field.
    /// </summary>
    NotSet, 

    /// <summary>
    /// The not null field.
    /// </summary>
    NotNull, 

    /// <summary>
    /// The can be null field.
    /// </summary>
    CanBeNull
  }

  /// <summary>
  /// Defines the <see cref="CodeAnnotation"/> class.
  /// </summary>
  public class CodeAnnotation
  {
    #region Constants and Fields

    /// <summary>
    /// The can be null field.
    /// </summary>
    private ITypeElement canBeNull;

    /// <summary>
    /// The not null field.
    /// </summary>
    private ITypeElement notNull;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeAnnotation"/> class. 
    /// Codes the annotation.
    /// </summary>
    /// <param name="typeMember">
    /// The type member.
    /// </param>
    public CodeAnnotation([NotNull] ITypeMemberDeclaration typeMember)
    {
      if (typeMember == null)
      {
        throw new ArgumentNullException("typeMember");
      }

      typeMember.GetPsiServices();

      this.Initialize(typeMember);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets a value indicating whether this instance is valid.
    /// </summary>
    public bool IsValid
    {
      get
      {
        return this.notNull != null && this.canBeNull != null;
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Gets the annotation.
    /// </summary>
    /// <param name="parameter">
    /// The parameter.
    /// </param>
    /// <returns>
    /// Returns the annotation.
    /// </returns>
    public CodeAnnotationAttribute GetAnnotation([NotNull] IParameterDeclaration parameter)
    {
      if (parameter == null)
      {
        throw new ArgumentNullException("parameter");
      }

      var attributesOwner = parameter.DeclaredElement as IAttributesOwner;

      return attributesOwner != null ? this.GetAnnotation(attributesOwner) : CodeAnnotationAttribute.Undefined;
    }

    /// <summary>
    /// Gets the annotation.
    /// </summary>
    /// <param name="typeMemberDeclaration">
    /// The parameter.
    /// </param>
    /// <returns>
    /// Returns the annotation.
    /// </returns>
    public CodeAnnotationAttribute GetAnnotation([NotNull] ITypeMemberDeclaration typeMemberDeclaration)
    {
      if (typeMemberDeclaration == null)
      {
        throw new ArgumentNullException("typeMemberDeclaration");
      }

      var attributesOwner = typeMemberDeclaration.DeclaredElement as IAttributesOwner;

      return attributesOwner != null ? this.GetAnnotation(attributesOwner) : CodeAnnotationAttribute.Undefined;
    }

    /// <summary>
    /// Gets the annotation.
    /// </summary>
    /// <param name="method">
    /// The method.
    /// </param>
    /// <returns>
    /// Returns the annotation.
    /// </returns>
    public CodeAnnotationAttribute GetAnnotation([NotNull] IMethodDeclaration method)
    {
      if (method == null)
      {
        throw new ArgumentNullException("method");
      }

      var attributesOwner = method.DeclaredElement as IAttributesOwner;

      return attributesOwner != null ? this.GetAnnotation(attributesOwner) : CodeAnnotationAttribute.Undefined;
    }

    /// <summary>
    /// Gets the annotation.
    /// </summary>
    /// <param name="property">
    /// The property.
    /// </param>
    /// <returns>
    /// Returns the annotation.
    /// </returns>
    public CodeAnnotationAttribute GetAnnotation([NotNull] IPropertyDeclaration property)
    {
      if (property == null)
      {
        throw new ArgumentNullException("property");
      }

      var attributesOwner = property.DeclaredElement as IAttributesOwner;

      return attributesOwner != null ? this.GetAnnotation(attributesOwner) : CodeAnnotationAttribute.Undefined;
    }

    /// <summary>
    /// Gets the annotation.
    /// </summary>
    /// <param name="indexer">
    /// The indexer.
    /// </param>
    /// <returns>
    /// Returns the annotation.
    /// </returns>
    public CodeAnnotationAttribute GetAnnotation([NotNull] IIndexerDeclaration indexer)
    {
      if (indexer == null)
      {
        throw new ArgumentNullException("indexer");
      }

      var attributesOwner = indexer.DeclaredElement as IAttributesOwner;

      return attributesOwner != null ? this.GetAnnotation(attributesOwner) : CodeAnnotationAttribute.Undefined;
    }

    /// <summary>
    /// Gets the state of the expression null reference.
    /// </summary>
    /// <param name="treeNode">
    /// The element.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="anchorStatement">
    /// The anchor statement.
    /// </param>
    /// <returns>
    /// Returns the expression null reference state.
    /// </returns>
    public CodeAnnotationAttribute GetExpressionNullReferenceState(ITreeNode treeNode, string name, IStatement anchorStatement)
    {
      var state = CodeAnnotationAttribute.Undefined;

      const string CookieName = "CodeAnnotations";

      var solution = treeNode.GetSolution();

      var cookie = solution.CreateTransactionCookie(DefaultAction.Rollback, CookieName);
      try
      {
        var psiServices = solution.GetPsiServices();

        Shell.Instance.GetComponent<UITaskExecutor>().SingleThreaded.ExecuteTask(CookieName, TaskCancelable.Yes, progress =>
        {
          progress.TaskName = CookieName;

          psiServices.PsiManager.DoTransaction(delegate { state = this.GetNullReferenceState(treeNode, name, anchorStatement); }, CookieName);

          cookie.Rollback();
        });
      }
      finally
      {
        if (cookie != null)
        {
          cookie.Dispose();
        }
      }

      return state;
    }

    /// <summary>
    /// Inspects the control graf.
    /// </summary>
    /// <param name="method">
    /// The method.
    /// </param>
    /// <returns>
    /// Returns the control graf.
    /// </returns>
    public CodeAnnotationAttribute InspectControlGraf([NotNull] IMethodDeclaration method)
    {
      if (method == null)
      {
        throw new ArgumentNullException("method");
      }

      return Inspect(method);
    }

    /// <summary>
    /// Inspects the control graf.
    /// </summary>
    /// <param name="constructor">
    /// The constructor.
    /// </param>
    /// <returns>
    /// Returns the control graf.
    /// </returns>
    public CodeAnnotationAttribute InspectControlGraf([NotNull] IConstructorDeclaration constructor)
    {
      if (constructor == null)
      {
        throw new ArgumentNullException("constructor");
      }

      if (!constructor.DeclaredElement.ReturnType.IsReferenceType())
      {
        return CodeAnnotationAttribute.Undefined;
      }

      return CodeAnnotationAttribute.NotNull;
    }

    /// <summary>
    /// Inspects the control graf.
    /// </summary>
    /// <param name="getter">
    /// The getter.
    /// </param>
    /// <returns>
    /// Returns the control graf.
    /// </returns>
    public CodeAnnotationAttribute InspectControlGraf([NotNull] IAccessorDeclaration getter)
    {
      if (getter == null)
      {
        throw new ArgumentNullException("getter");
      }

      return Inspect(getter);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inspects the specified function.
    /// </summary>
    /// <param name="function">
    /// The function.
    /// </param>
    /// <returns>
    /// Returns the code annotation attribute.
    /// </returns>
    private static CodeAnnotationAttribute Inspect([NotNull] ICSharpFunctionDeclaration function)
    {
      if (function == null)
      {
        throw new ArgumentNullException("function");
      }

      var project = function.GetProject();
      if (project == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var projectFile = project.ProjectFile;
      if (projectFile == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      if (!function.DeclaredElement.ReturnType.IsReferenceType())
      {
        return CodeAnnotationAttribute.Undefined;
      }

      // return CodeAnnotationAttribute.NotNull;
      AllNonQualifiedReferencesResolver.ProcessAll(function);
      var graf = CSharpControlFlowBuilder.Build(function);
      if (graf == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var result = graf.Inspect(HighlightingSettingsManager.Instance.GetValueAnalysisMode(projectFile));
      if (result == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      switch (result.SuggestReturnValueAnnotationAttribute)
      {
        case CSharpControlFlowNullReferenceState.NOT_NULL:
          return CodeAnnotationAttribute.NotNull;

        case CSharpControlFlowNullReferenceState.NULL:
          return CodeAnnotationAttribute.CanBeNull;

        case CSharpControlFlowNullReferenceState.MAY_BE_NULL:
          return CodeAnnotationAttribute.CanBeNull;
      }

      return CodeAnnotationAttribute.NotSet;
    }

    /// <summary>
    /// Codes the annotation attribute.
    /// </summary>
    /// <param name="attributesOwner">
    /// The attributes owner.
    /// </param>
    /// <returns>
    /// Returns the annotation attribute.
    /// </returns>
    private CodeAnnotationAttribute GetAnnotation([NotNull] IAttributesOwner attributesOwner)
    {
      if (attributesOwner == null)
      {
        throw new ArgumentNullException("attributesOwner");
      }

      var instances = attributesOwner.GetAttributeInstances(true);

      foreach (var attributeInstance in instances)
      {
        var shortName = attributeInstance.GetClrName().ShortName;
        if (shortName == "NotNullAttribute")
        {
          return CodeAnnotationAttribute.NotNull;
        }

        if (shortName == "CanBeNullAttribute")
        {
          return CodeAnnotationAttribute.CanBeNull;
        }
      }

      return CodeAnnotationAttribute.NotSet;
    }

    /// <summary>
    /// Gets the state of the expression null reference.
    /// </summary>
    /// <param name="treeNode">
    /// The element.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="anchorStatement">
    /// The anchor satement.
    /// </param>
    /// <returns>
    /// Returns the expression null reference state.
    /// </returns>
    private CodeAnnotationAttribute GetNullReferenceState(ITreeNode treeNode, string name, IStatement anchorStatement)
    {
      var block = treeNode.GetContainingNode<IBlock>(true);
      if (block == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var project = treeNode.GetProject();
      if (project == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var projectFile = project.ProjectFile;
      if (projectFile == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var factory = CSharpElementFactory.GetInstance(treeNode.GetPsiModule());
      var statement = factory.CreateStatement("if(" + name + " == null) { }");

      var ifStatement = block.AddStatementAfter(statement, (ICSharpStatement)anchorStatement) as IIfStatement;
      if (ifStatement == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var equalityExpression = ifStatement.Condition as IEqualityExpression;
      if (equalityExpression == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var referenceExpression = equalityExpression.LeftOperand as IReferenceExpression;
      if (referenceExpression == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var functionDeclaration = ifStatement.GetContainingNode<ICSharpFunctionDeclaration>(true);
      if (functionDeclaration == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var graf = CSharpControlFlowBuilder.Build(functionDeclaration);
      if (graf == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var inspect = graf.Inspect(HighlightingSettingsManager.Instance.GetValueAnalysisMode(projectFile));
      if (inspect == null)
      {
        return CodeAnnotationAttribute.Undefined;
      }

      var result = inspect.GetExpressionNullReferenceState(referenceExpression, false);
      switch (result)
      {
        case CSharpControlFlowNullReferenceState.NOT_NULL:
          return CodeAnnotationAttribute.NotNull;

        case CSharpControlFlowNullReferenceState.NULL:
          return CodeAnnotationAttribute.CanBeNull;

        case CSharpControlFlowNullReferenceState.MAY_BE_NULL:
          return CodeAnnotationAttribute.CanBeNull;
      }

      return CodeAnnotationAttribute.Undefined;
    }

    /// <summary>
    /// Initializes the specified type member.
    /// </summary>
    /// <param name="typeMember">
    /// The type member.
    /// </param>
    private void Initialize([NotNull] ITypeMemberDeclaration typeMember)
    {
      if (typeMember == null)
      {
        throw new ArgumentNullException("typeMember");
      }

      var sourceFile = typeMember.GetSourceFile();
      if (sourceFile == null)
      {
        return;
      }

      var psiServices = typeMember.GetPsiServices();
      if (psiServices == null)
      {
        throw new InvalidOperationException("psiServices");
      }

      var cache = psiServices.GetCodeAnnotationsCache();

      this.notNull = cache.GetAttributeTypeForElement(typeMember, CodeAnnotationsCache.NotNullAttributeShortName);
      this.canBeNull = cache.GetAttributeTypeForElement(typeMember, CodeAnnotationsCache.CanBeNullAttributeShortName);
    }

    #endregion
  }
}
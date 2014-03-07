// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnnotationModel.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the <see cref="CodeAnnotationModel" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeAnnotationPack.CodeAnnotations
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;

  /// <summary>Defines the <see cref="CodeAnnotationModel"/> class.</summary>
  public class CodeAnnotationModel
  {
    #region Fields

    /// <summary>The parameters field.</summary>
    private readonly List<ParameterDescriptor> parameters = new List<ParameterDescriptor>();

    #endregion

    #region Constructors and Destructors

    /// <summary>Initializes a new instance of the <see cref="CodeAnnotationModel"/> class.</summary>
    /// <param name="publicAssertionCode">The public assertion code.</param>
    /// <param name="nonPublicAssertionCode">The non public assertion code.</param>
    /// <param name="alternativeAssertions"></param>
    public CodeAnnotationModel([NotNull] string publicAssertionCode, [NotNull] string nonPublicAssertionCode, IEnumerable<string> alternativeAssertions)
    {
      if (publicAssertionCode == null)
      {
        throw new ArgumentNullException("publicAssertionCode");
      }

      if (nonPublicAssertionCode == null)
      {
        throw new ArgumentNullException("nonPublicAssertionCode");
      }

      this.PublicAssertionCode = publicAssertionCode;
      this.NonPublicAssertionCode = nonPublicAssertionCode;
      this.AlternativeAssertions = alternativeAssertions;

      this.GenerateAssertions = true;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the alternative assertions.
    /// </summary>
    /// <value>The alternative assertions.</value>
    [NotNull]
    public IEnumerable<string> AlternativeAssertions { get; private set; }

    /// <summary>
    /// Gets or sets the applied return.
    /// </summary>
    /// <value>The applied return.</value>
    public CodeAnnotationAttribute AppliedReturn { get; set; }

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    /// <value>The body.</value>
    [CanBeNull]
    public IBlock Body { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is annotated.
    /// </summary>
    public bool CanFix
    {
      get
      {
        if (this.ExpectedReturn != this.AppliedReturn && this.ExpectedReturn != CodeAnnotationAttribute.Undefined && this.ExpectedReturn != CodeAnnotationAttribute.NotSet)
        {
          return true;
        }

        foreach (var descriptor in this.Parameters)
        {
          if (descriptor.ExpectedAttribute != descriptor.AppliedAttribute && descriptor.ExpectedAttribute != CodeAnnotationAttribute.Undefined && descriptor.ExpectedAttribute != CodeAnnotationAttribute.NotSet)
          {
            return true;
          }

          if (descriptor.ExpectsAssertion && descriptor.Assertion == null)
          {
            return true;
          }
        }

        return false;
      }
    }

    /// <summary>
    /// Gets or sets the code annotation.
    /// </summary>
    /// <value>The code annotation.</value>
    [NotNull]
    public CodeAnnotation CodeAnnotation { get; set; }

    /// <summary>
    /// Gets or sets the expected return.
    /// </summary>
    /// <value>The expected return.</value>
    public CodeAnnotationAttribute ExpectedReturn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [generate assertions].
    /// </summary>
    /// <value><c>true</c> if [generate assertions]; otherwise, <c>false</c>.</value>
    public bool GenerateAssertions { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is annotated.
    /// </summary>
    public bool IsAnnotated
    {
      get
      {
        if (this.ExpectedReturn != this.AppliedReturn && this.ExpectedReturn != CodeAnnotationAttribute.Undefined && this.ExpectedReturn != CodeAnnotationAttribute.NotSet)
        {
          return false;
        }

        foreach (var descriptor in this.Parameters)
        {
          if (descriptor.ExpectedAttribute != descriptor.AppliedAttribute)
          {
            return false;
          }

          if (descriptor.ExpectsAssertion && descriptor.Assertion == null)
          {
            return false;
          }
        }

        return true;
      }
    }

    /// <summary>
    /// Gets or sets the non public assertion code.
    /// </summary>
    /// <value>The non public assertion code.</value>
    public string NonPublicAssertionCode { get; set; }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    public IEnumerable<ParameterDescriptor> Parameters
    {
      get
      {
        return this.parameters;
      }
    }

    /// <summary>
    /// Gets or sets the public assertion code.
    /// </summary>
    /// <value>The public assertion code.</value>
    public string PublicAssertionCode { get; set; }

    /// <summary>
    /// Gets or sets the type memeber.
    /// </summary>
    /// <value>The type memeber.</value>
    public ITypeMemberDeclaration TypeMember { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>Inspects this instance.</summary>
    /// <param name="method">The method.</param>
    public void Inspect([NotNull] IMethodDeclaration method)
    {
      this.CodeAnnotation = new CodeAnnotation(method);
      if (!this.CodeAnnotation.IsValid)
      {
        return;
      }

      if (method.IsExtern)
      {
        return;
      }

      var body = method.Body;

      this.TypeMember = method;
      this.Body = body;

      this.AppliedReturn = this.CodeAnnotation.GetAnnotation(method);

      if (this.AppliedReturn != CodeAnnotationAttribute.Undefined && this.AppliedReturn != CodeAnnotationAttribute.NotSet)
      {
        this.ExpectedReturn = this.AppliedReturn;
      }
      else
      {
        var attribute = this.CodeAnnotation.InspectControlGraf(method);
        if (attribute != CodeAnnotationAttribute.Undefined && attribute != CodeAnnotationAttribute.NotSet)
        {
          this.ExpectedReturn = attribute;
        }
        else if (attribute == CodeAnnotationAttribute.NotSet)
        {
          this.ExpectedReturn = CodeAnnotationAttribute.NotNull;
        }
      }

      this.BuildParameters(method, method.IsAbstract);
      if (body != null)
      {
        this.BuildAssertions(body);
      }
    }

    /// <summary>Inspects the specified method.</summary>
    /// <param name="constructor">The method.</param>
    public void Inspect([NotNull] IConstructorDeclaration constructor)
    {
      this.CodeAnnotation = new CodeAnnotation(constructor);
      if (!this.CodeAnnotation.IsValid)
      {
        return;
      }

      var body = constructor.Body;
      if (body == null)
      {
        return;
      }

      this.TypeMember = constructor;
      this.Body = body;

      this.AppliedReturn = CodeAnnotationAttribute.Undefined;
      this.ExpectedReturn = CodeAnnotationAttribute.Undefined;

      this.BuildParameters(constructor, false);
      this.BuildAssertions(body);
    }

    /// <summary>Inspects the specified method.</summary>
    /// <param name="property">The method.</param>
    public void Inspect([NotNull] IPropertyDeclaration property)
    {
      this.CodeAnnotation = new CodeAnnotation(property);
      if (!this.CodeAnnotation.IsValid)
      {
        return;
      }

      if (property.IsExtern)
      {
        return;
      }

      this.TypeMember = property;
      this.AppliedReturn = this.CodeAnnotation.GetAnnotation(property);

      var getter = property.AccessorDeclarations.FirstOrDefault(a => a.Kind == AccessorKind.GETTER);
      var setter = property.AccessorDeclarations.FirstOrDefault(a => a.Kind == AccessorKind.SETTER);

      if (this.AppliedReturn != CodeAnnotationAttribute.Undefined && this.AppliedReturn != CodeAnnotationAttribute.NotSet)
      {
        this.ExpectedReturn = this.AppliedReturn;
      }
      else if (getter != null)
      {
        var attribute = this.CodeAnnotation.InspectControlGraf(getter);
        if (attribute != CodeAnnotationAttribute.Undefined && attribute != CodeAnnotationAttribute.NotSet)
        {
          this.ExpectedReturn = attribute;
        }
        else if (attribute == CodeAnnotationAttribute.NotSet)
        {
          this.ExpectedReturn = CodeAnnotationAttribute.NotNull;
        }
      }

      if (getter != null)
      {
        this.BuildValueParameter(this.TypeMember, getter, setter);
      }

      if (setter == null)
      {
        return;
      }

      var body = setter.Body;
      if (body == null)
      {
        return;
      }

      this.Body = body;

      // this.BuildParameters(setter, false);
      this.BuildAssertions(body);
    }

    /// <summary>Inspects the specified indexer.</summary>
    /// <param name="indexer">The indexer.</param>
    public void Inspect([NotNull] IIndexerDeclaration indexer)
    {
      this.CodeAnnotation = new CodeAnnotation(indexer);
      if (!this.CodeAnnotation.IsValid)
      {
        return;
      }

      if (indexer.IsAbstract || indexer.IsExtern)
      {
        return;
      }

      this.TypeMember = indexer;
      this.AppliedReturn = this.CodeAnnotation.GetAnnotation(indexer);

      var getter = indexer.AccessorDeclarations.FirstOrDefault(a => a.Kind == AccessorKind.GETTER);
      var setter = indexer.AccessorDeclarations.FirstOrDefault(a => a.Kind == AccessorKind.SETTER);

      if (this.AppliedReturn != CodeAnnotationAttribute.Undefined && this.AppliedReturn != CodeAnnotationAttribute.NotSet)
      {
        this.ExpectedReturn = this.AppliedReturn;
      }
      else if (getter != null)
      {
        var attribute = this.CodeAnnotation.InspectControlGraf(getter);
        if (attribute != CodeAnnotationAttribute.Undefined && attribute != CodeAnnotationAttribute.NotSet)
        {
          this.ExpectedReturn = attribute;
        }
      }

      if (getter != null)
      {
        this.BuildValueParameter(this.TypeMember, getter, setter);
      }

      if (setter == null)
      {
        return;
      }

      var body = setter.Body;
      if (body == null)
      {
        return;
      }

      this.Body = body;

      // this.BuildParameters(setter, false);
      this.BuildAssertions(body);
    }

    #endregion

    #region Methods

    /// <summary>Gets the assertion statements.</summary>
    /// <param name="block">The block.</param>
    private void BuildAssertions([NotNull] IBlock block)
    {
      if (block == null)
      {
        throw new ArgumentNullException("block");
      }

      var parameterCount = this.parameters.Count();
      var count = 0;

      foreach (var statement in block.Statements)
      {
        var text = statement.GetText().Trim();

        var descriptor = this.GetParameterDescriptor(text);
        if (descriptor == null)
        {
          continue;
        }

        descriptor.Assertion = statement;
        descriptor.Code = text;

        count++;
        if (count == parameterCount)
        {
          return;
        }
      }
    }

    /// <summary>Builds the parameters.</summary>
    /// <param name="parametersOwner">The method.</param>
    /// <param name="isAbstract">if set to <c>true</c> [is abstract].</param>
    private void BuildParameters(IParametersOwnerDeclaration parametersOwner, bool isAbstract)
    {
      foreach (var parameter in parametersOwner.ParameterDeclarations)
      {
        if (!parameter.Type.IsReferenceType())
        {
          continue;
        }

        var expectsAssertion = false;
        var assertionCode = string.Empty;
        var appliedAttribute = this.CodeAnnotation.GetAnnotation(parameter);
        var expectedAttribute = CodeAnnotationAttribute.NotNull;

        if (appliedAttribute == CodeAnnotationAttribute.CanBeNull)
        {
          expectedAttribute = CodeAnnotationAttribute.CanBeNull;
        }
        else if (parameter.DeclaredElement != null && parameter.DeclaredElement.Kind == ParameterKind.OUTPUT)
        {
          appliedAttribute = CodeAnnotationAttribute.Undefined;
          expectedAttribute = CodeAnnotationAttribute.Undefined;
        }
        else
        {
          if (this.GenerateAssertions)
          {
            assertionCode = this.GetCode();
            if (!string.IsNullOrEmpty(assertionCode))
            {
              assertionCode = string.Format(assertionCode, parameter.DeclaredName).Trim();
              expectsAssertion = true;
            }
          }
        }

        if (isAbstract)
        {
          expectsAssertion = false;
        }

        var descriptor = new ParameterDescriptor(parameter, expectedAttribute, appliedAttribute, expectsAssertion, assertionCode);
        this.parameters.Add(descriptor);
      }
    }

    /// <summary>Builds the value parameter.</summary>
    /// <param name="typeMember">The type member.</param>
    /// <param name="getter">The getter.</param>
    /// <param name="setter">The setter.</param>
    private void BuildValueParameter([NotNull] ITypeMemberDeclaration typeMember, [NotNull] IAccessorDeclaration getter, [CanBeNull] IAccessorDeclaration setter)
    {
      if (typeMember == null)
      {
        throw new ArgumentNullException("typeMember");
      }

      if (getter == null)
      {
        throw new ArgumentNullException("getter");
      }

      var returnType = getter.DeclaredElement.ReturnType;
      if (!returnType.IsReferenceType())
      {
        return;
      }

      var expectsAssertion = false;
      var assertionCode = string.Empty;
      var appliedAttribute = this.AppliedReturn;
      var expectedAttribute = this.ExpectedReturn;

      if (appliedAttribute == CodeAnnotationAttribute.CanBeNull)
      {
      }
      else if (appliedAttribute == CodeAnnotationAttribute.NotNull || expectedAttribute == CodeAnnotationAttribute.NotNull)
      {
        if (this.GenerateAssertions)
        {
          if (setter != null && setter.Body != null)
          {
            assertionCode = this.GetCode();
            if (!string.IsNullOrEmpty(assertionCode))
            {
              assertionCode = string.Format(assertionCode, "value").Trim();
              expectsAssertion = true;
            }
          }
        }
      }

      var descriptor = new ParameterDescriptor(expectedAttribute, appliedAttribute, expectsAssertion, assertionCode);
      this.parameters.Add(descriptor);
    }

    /// <summary>Gets the code.</summary>
    /// <returns>Returns the code.</returns>
    [NotNull]
    private string GetCode()
    {
      return this.TypeMember.DeclaredElement.GetAccessRights() == AccessRights.PUBLIC ? this.PublicAssertionCode : this.NonPublicAssertionCode;
    }

    /// <summary>Gets the parameter descriptor.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Returns the parameter descriptor.</returns>
    [CanBeNull]
    private ParameterDescriptor GetParameterDescriptor([NotNull] string text)
    {
      foreach (var parameter in this.Parameters)
      {
        if (string.IsNullOrEmpty(parameter.Code))
        {
          continue;
        }

        if (parameter.Code.EndsWith(text) || text.EndsWith(parameter.Code))
        {
          return parameter;
          break;
        }

        var parameterDeclaration = parameter.Parameter;
        if (parameterDeclaration == null)
        {
          continue;
        }

        var name = parameterDeclaration.DeclaredName;

        foreach (var alternativeAssertion in this.AlternativeAssertions)
        {
          var code = string.Format(alternativeAssertion, name);
          if (code.EndsWith(text) || text.EndsWith(code))
          {
            return parameter;
          }
        }
      }

      return null;
    }

    #endregion
  }
}
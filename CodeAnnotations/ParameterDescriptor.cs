namespace CodeAnnotationPack.CodeAnnotations
{
  using System;
  using JetBrains.Annotations;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>Defines the <see cref="ParameterDescriptor"/> class.</summary>
  public class ParameterDescriptor
  {
    #region Constructors and Destructors

    /// <summary>Initializes a new instance of the <see cref="ParameterDescriptor"/> class.</summary>
    /// <param name="expectedAttribute">The expected attribute.</param>
    /// <param name="appliedAttribute">The applied attribute.</param>
    /// <param name="expectsAssertion">if set to <c>true</c> [expects assertion].</param>
    /// <param name="assertionCode">The assertion code.</param>
    public ParameterDescriptor(CodeAnnotationAttribute expectedAttribute, CodeAnnotationAttribute appliedAttribute, bool expectsAssertion, [NotNull] string assertionCode)
    {
      this.ExpectedAttribute = expectedAttribute;
      this.AppliedAttribute = appliedAttribute;
      this.ExpectsAssertion = expectsAssertion;
      this.Code = assertionCode;
    }

    /// <summary>Initializes a new instance of the <see cref="ParameterDescriptor"/> class.</summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="expectedAttribute">The expected attribute.</param>
    /// <param name="appliedAttribute">The applied attribute.</param>
    /// <param name="expectsAssertion">if set to <c>true</c> [expects assertion].</param>
    /// <param name="assertionCode">The assertion code.</param>
    public ParameterDescriptor([NotNull] IParameterDeclaration parameter, CodeAnnotationAttribute expectedAttribute, CodeAnnotationAttribute appliedAttribute, bool expectsAssertion, [NotNull] string assertionCode)
    {
      if (parameter == null)
      {
        throw new ArgumentNullException("parameter");
      }

      this.Parameter = parameter;
      this.ExpectedAttribute = expectedAttribute;
      this.AppliedAttribute = appliedAttribute;
      this.ExpectsAssertion = expectsAssertion;
      this.Code = assertionCode;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the applied attribute.
    /// </summary>
    /// <value>The applied attribute.</value>
    public CodeAnnotationAttribute AppliedAttribute { get; set; }

    /// <summary>
    /// Gets or sets the assertion.
    /// </summary>
    /// <value>The assertion.</value>
    [CanBeNull]
    public IStatement Assertion { get; set; }

    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>The code.</value>
    [NotNull]
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the expected attribute.
    /// </summary>
    /// <value>The expected attribute.</value>
    public CodeAnnotationAttribute ExpectedAttribute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [expects assertion].
    /// </summary>
    /// <value><c>true</c> if [expects assertion]; otherwise, <c>false</c>.</value>
    public bool ExpectsAssertion { get; set; }

    /// <summary>
    /// Gets the parameter.
    /// </summary>
    /// <value>The parameter.</value>
    [CanBeNull]
    public IParameterDeclaration Parameter { get; private set; }

    #endregion
  }
}
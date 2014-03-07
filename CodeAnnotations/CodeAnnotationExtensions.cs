namespace CodeAnnotationPack.CodeAnnotations
{
  using CodeAnnotationPack.Attributes;
  using JetBrains.ReSharper.Psi.CodeAnnotations;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>Defines the <see cref="CodeAnnotationExtensions"/> class.</summary>
  public static class CodeAnnotationExtensions
  {
    #region Public Methods and Operators

    /// <summary>Annotates the specified tree node.</summary>
    /// <param name="treeNode">The tree node.</param>
    /// <param name="attribute">The attribute.</param>
    public static void Annotate(this ITreeNode treeNode, CodeAnnotationAttribute attribute)
    {
      var owner = treeNode as IAttributesOwnerDeclaration;
      if (owner == null)
      {
        return;
      }

      string attributeName;
      switch (attribute)
      {
        case CodeAnnotationAttribute.NotNull:
          attributeName = CodeAnnotationsCache.NotNullAttributeShortName;
          break;
        case CodeAnnotationAttribute.CanBeNull:
          attributeName = CodeAnnotationsCache.CanBeNullAttributeShortName;
          break;
        default:
          return;
      }

      AttributesHelper.SetAttribute(owner, attributeName);
    }

    #endregion
  }
}
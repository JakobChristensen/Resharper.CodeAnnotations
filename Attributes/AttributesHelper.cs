namespace CodeAnnotationPack.Attributes
{
  using System;
  using JetBrains.Annotations;
  using JetBrains.ReSharper.Psi.CodeAnnotations;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>Defines the <see cref="AttributesHelper"/> class.</summary>
  internal static class AttributesHelper
  {
    #region Public Methods and Operators

    /// <summary>Sets the attribute.</summary>
    /// <param name="owner">The owner.</param>
    /// <param name="attributeShortName">Short name of the attribute.</param>
    public static void SetAttribute([NotNull] IAttributesOwnerDeclaration owner, [NotNull] string attributeShortName)
    {
      if (owner == null)
      {
        throw new ArgumentNullException("owner");
      }

      if (!owner.IsValid())
      {
        return;
      }

      if (attributeShortName == null)
      {
        throw new ArgumentNullException("attributeShortName");
      }

      var psiServices = owner.GetPsiServices();
      if (psiServices == null)
      {
        throw new InvalidOperationException("psiServices");
      }

      var cache = psiServices.GetCodeAnnotationsCache();
      if (cache == null)
      {
        return;
      }

      var attributeTypeElement = cache.GetAttributeTypeForElement(owner, attributeShortName);
      if (attributeTypeElement == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(owner.GetPsiModule());

      var attribute = factory.CreateAttribute(attributeTypeElement);
      owner.AddAttributeAfter(attribute, null);
    }

    #endregion
  }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnnotationsOptionsPage.cs" company="Jakob Christensen">
//   Copyright (C) by Jakob Christensen
// </copyright>
// <summary>
//   The value analysis options page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeAnnotationPack.Options
{
  using System.Windows.Forms;
  using JetBrains.Application.Settings;
  using JetBrains.DataFlow;
  using JetBrains.UI.CrossFramework;
  using JetBrains.UI.Options;
  using JetBrains.UI.Resources;

  /// <summary>
  /// The value analysis options page.
  /// </summary>
  [OptionsPage(PageName, "Code Annotation Pack", typeof(OptionsThemedIcons.Plugins), ParentId = "CodeInspection", Sequence = 4.7)]
  public partial class CodeAnnotationsOptionsPage : UserControl, IOptionsPage
  {
    #region Constants and Fields

    /// <summary>
    /// The page name.
    /// </summary>
    private const string PageName = "CodeAnnotationPack.CodeAnnotationsPage";

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeAnnotationsOptionsPage"/> class.
    /// </summary>
    /// <param name="lifetime">
    /// The lifetime.
    /// </param>
    /// <param name="settings">
    /// The settings.
    /// </param>
    public CodeAnnotationsOptionsPage(Lifetime lifetime, OptionsSettingsSmartContext settings)
    {
      this.InitializeComponent();

      settings.SetBinding(lifetime, (CodeAnnotationPackSettings s) => s.PublicAssertion, WinFormsProperty.Create(lifetime, this.PublicAssertion, box => box.Text, true));
      settings.SetBinding(lifetime, (CodeAnnotationPackSettings s) => s.NonPublicAssertion, WinFormsProperty.Create(lifetime, this.NonPublicAssertion, box => box.Text, true));
      settings.SetBinding(lifetime, (CodeAnnotationPackSettings s) => s.AlternativeAssertions, WinFormsProperty.Create(lifetime, this.AlternativeAssertions, box => box.Text, true));
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the control to be shown as page.
    /// </summary>
    /// <remarks>
    /// May be <c>Null</c> if the page does not have any UI.
    /// </remarks>
    /// <value>
    /// </value>
    public EitherControl Control
    {
      get
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the ID of this option page.
    /// <see cref="T:JetBrains.UI.Options.IOptionsDialog"/> or <see cref="T:JetBrains.UI.Options.OptionsPageDescriptor"/> could be used to retrieve the <see cref="T:JetBrains.UI.Options.OptionsManager"/> out of it.
    /// </summary>
    /// <value>
    /// </value>
    public string Id
    {
      get
      {
        return PageName;
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Invoked when OK button in the options dialog is pressed
    /// If the page returns <c>false</c>, the the options dialog won't be closed, and focus
    /// will be put into this page
    /// </summary>
    /// <returns>
    /// <c>True</c>, if OK.
    /// </returns>
    public bool OnOk()
    {
      return true;
    }

    /// <summary>
    /// Check if the settings on the page are consistent, and page could be closed
    /// </summary>
    /// <returns>
    /// <c>true</c> if page data is consistent
    /// </returns>
    public bool ValidatePage()
    {
      return true;
    }

    #endregion
  }
}
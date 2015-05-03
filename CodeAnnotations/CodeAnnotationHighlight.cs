// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConvertPathToIdHighlight.cs" company="Sitecore A/S">
//   Copyright (C) by Sitecore A/S
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeAnnotationPack.CodeAnnotations
{
  using JetBrains.Annotations;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
  using JetBrains.ReSharper.Feature.Services.Daemon;

	/// <summary>Defines the <see cref="CodeAnnotationHighlight"/> class.</summary>
  [ConfigurableSeverityHighlighting("CodeAnnotationAnalyzer", "CSHARP", OverlapResolve = OverlapResolveKind.WARNING, ToolTipFormatString = "Add NotNull or CanBeNull attributes [Code Annotation Pack]")]
  public class CodeAnnotationHighlight : CSharpHighlightingBase, IHighlighting
  {
    #region Constructors and Destructors

    /// <summary>Initializes a new instance of the <see cref="CodeAnnotationHighlight"/> class.</summary>
    /// <param name="model">The literal.</param>
    /// <param name="range"></param>
    public CodeAnnotationHighlight([NotNull] CodeAnnotationModel model, DocumentRange range)
    {
      this.Model = model;
      this.Range = range;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlightingAttributeBase.ShowToolTipInStatusBar" /> is <c>true</c>)
    /// </summary>
    /// <value>The error stripe tool tip.</value>
    [NotNull]
    public string ErrorStripeToolTip
    {
      get
      {
        return this.ToolTip;
      }
    }

    /// <summary>
    /// Gets the literal expression.
    /// </summary>
    /// <value>The literal expression.</value>
    [NotNull]
    public CodeAnnotationModel Model
    {
      get;
      private set;
    }

    public DocumentRange Range { get; set; }

    /// <summary>
    /// Gets the offset from the Range.StartOffset to set the cursor to when navigating
    /// to this highlighting. Usually returns <c>0</c>
    /// </summary>
    /// <value>The navigation offset patch.</value>
    public int NavigationOffsetPatch
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlightingAttributeBase.ShowToolTipInStatusBar" /> is <c>true</c>)
    /// To override the default mechanism of tooltip, mark the implementation class with
    /// <see cref="T:JetBrains.ReSharper.Daemon.DaemonTooltipProviderAttribute" /> attribute, and then this property will not be called
    /// </summary>
    /// <value>The tool tip.</value>
    [NotNull]
    public string ToolTip
    {
      get
      {
        return "Add NotNull or CanBeNull attributes [Code Annotation Pack]";
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>Returns true if data (PSI, text ranges) associated with highlighting is valid</summary>
    /// <returns>The <see cref="bool"/>.</returns>
    public override bool IsValid()
    {
      return this.Range.IsValid();
    }

    #endregion

    #region Explicit Interface Methods

    /// <summary>
    /// Calculates the range.
    /// </summary>
    /// <returns>Returns the document range.</returns>
    public DocumentRange CalculateRange()
    {
      return this.Range;
    }

    #endregion
  }
}
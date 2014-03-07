// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnnotationPackSettings.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the  class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CodeAnnotationPack.Options
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using JetBrains.Application.Settings;

  /// <summary>
  /// Defines the <see cref="CodeAnnotationPackSettings"/> class.
  /// </summary>
  [SettingsKey(typeof(EnvironmentSettings), "Code Annotation Pack Settings")]
  public class CodeAnnotationPackSettings
  {
    #region Static Fields

    /// <summary>
    /// The chars
    /// </summary>
    public static readonly char[] AlternativeAssertionsChars =
    {
      '\n'
    };

    #endregion

    #region Fields

    /// <summary>
    /// The alternative assertions
    /// </summary>
    private string alternativeAssertions;

    /// <summary>
    /// The alternative code
    /// </summary>
    private IEnumerable<string> alternativeCode;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the alternative assertions.
    /// </summary>
    /// <value>The alternative assertions.</value>
    [SettingsEntry("", "AlternativeAssertions")]
    public string AlternativeAssertions
    {
      get
      {
        return this.alternativeAssertions;
      }

      set
      {
        this.alternativeAssertions = value;
        this.alternativeCode = null;
      }
    }

    /// <summary>
    /// Gets the alternative code.
    /// </summary>
    /// <value>The alternative code.</value>
    [NotNull]
    public IEnumerable<string> AlternativeAssertionsList
    {
      get
      {
        if (this.alternativeCode == null)
        {
          if (!string.IsNullOrEmpty(this.AlternativeAssertions))
          {
            this.alternativeCode = new List<string>(this.AlternativeAssertions.Split(AlternativeAssertionsChars, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()));
          }
          else
          {
            this.alternativeCode = Enumerable.Empty<string>();
          }
        }

        return this.alternativeCode;
      }
    }

    /// <summary>
    /// Gets or sets the max templates.
    /// </summary>
    /// <value>The max templates.</value>
    [SettingsEntry("", "NonPublicAssertion")]
    public string NonPublicAssertion { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [file saves].
    /// </summary>
    /// <value><c>true</c> if [file saves]; otherwise, <c>false</c>.</value>
    [SettingsEntry("", "PublicAssertion")]
    public string PublicAssertion { get; set; }

    #endregion
  }
}
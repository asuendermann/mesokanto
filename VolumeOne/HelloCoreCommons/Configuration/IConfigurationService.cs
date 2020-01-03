using Microsoft.Extensions.Configuration;

namespace HelloCoreCommons.Configuration {
    public interface IConfigurationService {
        IConfigurationRoot Configuration { get; }

        string ProjectConnectionString { get; }

        /// <summary>
        ///     reads the specified key from the ApplicationSettings section.
        ///     Redirects to GetValue(section, key, defaultValue).
        /// </summary>
        /// <param name="key">the key of the parameter value.</param>
        /// <param name="defaultValue">the default value for the property.</param>
        /// <returns>the value from the configuration if present, else the default value if specified, or finally an empty string.</returns>
        string GetAppSetting(string key, string defaultValue = null);

        /// <summary>
        ///     reads the specified key from the specified section.
        /// </summary>
        /// <param name="section">the section where to reads the parameter value.</param>
        /// <param name="key">the key of the parameter value.</param>
        /// <param name="defaultValue">the default value for the property.</param>
        /// <returns>the value from the configuration if present, else the default value if specified, or finally an empty string.</returns>
        string GetSectionValue(string section, string key, string defaultValue = null);
    }
}
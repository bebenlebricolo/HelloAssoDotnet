using System.Text.RegularExpressions;

namespace HelloAssoDotnet.Models.Utils;

/// <summary>
/// Performs tasks such as in-place env var substitution in strings.
/// </summary>
public static class EnvVarResolver
{
    /// <summary>
    /// Substitute environment variables in place
    /// </summary>
    /// <param name="subjectString">input string with env var refs</param>
    /// <returns>Transformed string (in-place substitution)</returns>
    public static string SusbtituteEnvInString(string subjectString)
    {
        string resultingString = subjectString;
        const string regexPattern = """(\$\w*)""";

        // If we have environment variables, perform in-place substitution
        var regex = new Regex(regexPattern);
        var matches = regex.Matches(subjectString!);
        if (matches.Any())
        {
            List<Tuple<string, Match>> envVars = new();
            foreach (Match match in matches)
            {
                var content = match.Groups[1].Value;
                var envVar = Environment.GetEnvironmentVariable(content.Replace("$", ""));
                if (envVar != null)
                {
                    envVars.Add(new Tuple<string, Match>(envVar, match));
                }
            }

            // Perform in-place replacement
            foreach (var envVar in envVars)
            {
                var matchVal = envVar.Item2.Groups[1].Value;
                resultingString = resultingString.Replace(matchVal, envVar.Item1);
            }
        }
        return resultingString;
    }
}

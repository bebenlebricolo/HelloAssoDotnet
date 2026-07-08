using System;
using System.Collections.Generic;
using System.Text;

namespace HelloAssoDotnet.Models.Api.Forms
{
    /// <summary>
    /// Depicts a logo used in the <see cref="FormLightModel"/>
    /// </summary>
    public record Logo
    {
        /// <summary>
        /// Logo filename (seems to be a full URL anyway)
        /// </summary>
        public string FileName { get; set; } = "";

        /// <summary>
        /// Public Url for the Form logo
        /// </summary>
        public string PublicUrl { get; set; } = "";
    }
}

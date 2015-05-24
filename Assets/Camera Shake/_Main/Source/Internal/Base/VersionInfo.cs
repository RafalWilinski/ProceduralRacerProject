//
// VersionInfo.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2014 Thinksquirrel Software, LLC
//
using System.Text;

namespace Thinksquirrel.CameraShakeInternal
{
    /// <summary>
    /// Contains version information for Camera Shake.
    /// </summary>
    static class VersionInfo
    {
        const int _major = 1;
        const int _minor = 4;
        const int _incremental = 0;
        const int _revision = 1;
#if CAMERASHAKE_BETA
        const string _revisionString = "b";
#else
        const string _revisionString = "f";
#endif
#if CAMERASHAKE_BETA
        static readonly bool _isBeta = true;
#else
        static readonly bool _isBeta = false;
#endif
        /// <summary>
        /// Gets the incremental version number.
        /// </summary>
        public static int incremental
        {
            get
            {
                return _incremental;
            }
        }

        /// <summary>
        /// Gets the major version number.
        /// </summary>
        public static int major
        {
            get
            {
                return _major;
            }
        }

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        public static int minor
        {
            get
            {
                return _minor;
            }
        }

        /// <summary>
        /// Gets the revision version number.
        /// </summary>
        public static int revision
        {
            get
            {
                return _revision;
            }
        }

        /// <summary>
        /// Whether or not the current Camera Shake build is a beta.
        /// </summary>
        public static bool isBeta
        {
            get
            {
                return _isBeta;
            }
        }

        /// <summary>
        /// Gets the version of Camera Shake.
        /// </summary>
        public static string version
        {
            get
            {
                return string.Format("{0}.{1}.{2}{3}{4}", _major, _minor, _incremental, _revisionString, _revision);
            }
        }

        /// <summary>
        /// Gets the current Camera Shake license, in human-readable form.
        /// </summary>
        public static string license
        {
            get
            {
                return "Camera Shake";
            }
        }

        /// <summary>
        /// Gets the copyright message.
        /// </summary>
        public static string copyright
        {
            get
            {
                return "(c) 2012-2014 Thinksquirrel Software, LLC.";
            }
        }

        /// <summary>
        /// Gets the Camera Shake reference manual URL.
        /// </summary>
        public static string ReferenceManualUrl()
        {
            return string.Format("https://docs.thinksquirrel.com/camera-shake/{0}/reference/", version);
        }

        /// <summary>
        /// Gets the Camera Shake support forum URL.
        /// </summary>
        public static string SupportForumUrl()
        {
            return "https://support.thinksquirrel.com/hc/communities/public/topics/200145264-Camera-Shake";
        }

        /// <summary>
        /// Gets the Camera Shake reference manual archive URL.
        /// </summary>
        public static string ArchiveUrl()
        {
            return string.Format("https://docs.thinksquirrel.com/camera-shake/archives/{0}.zip", version);
        }

        /// <summary>
        /// Gets the Camera Shake manual URL for the specified component.
        /// </summary>
        public static string ComponentUrl(System.Type type, bool includeBaseUrl = true)
        {
            string fullTypeName = type.ToString();

            if (!fullTypeName.Contains("Thinksquirrel"))
            {
                var attribute = System.Attribute.GetCustomAttribute(type, typeof(CameraShakeDocumentationName));

                if (attribute != null)
                {
                    var typedAttribute = (CameraShakeDocumentationName)attribute;

                    fullTypeName = typedAttribute.name;
                }
                else
                {
                    return null;
                }
            }

            string[] names = fullTypeName.Split('.');
            fullTypeName = names [names.Length - 1];
            var sb = new StringBuilder();
            sb.Append(fullTypeName.ToLowerInvariant());
            sb.Append(".html");

            return includeBaseUrl ? ReferenceManualUrl() + sb : sb.ToString();
        }

        /// <summary>
        /// Gets the Camera Shake Scripting API URL for the specified type.
        /// </summary>
        public static string ScriptingUrl(System.Type type, bool includeBaseUrl = true)
        {
            string fullTypeName = type.ToString();

            if (!fullTypeName.Contains("Thinksquirrel"))
            {
                var attribute = System.Attribute.GetCustomAttribute(type, typeof(CameraShakeDocumentationName));

                if (attribute != null)
                {
                    var typedAttribute = (CameraShakeDocumentationName)attribute;

                    fullTypeName = typedAttribute.name;
                }
                else
                {
                    return null;
                }
            }

            fullTypeName = fullTypeName.Replace(".", "_1_1_");
            var sb = new StringBuilder();
            sb.Append("class_");
            sb.Append(HumanizeString(fullTypeName).Replace(' ', '_').ToLowerInvariant());
            sb.Append(".html");

            return includeBaseUrl ? ReferenceManualUrl() + sb : sb.ToString();
        }

        /// <summary>
        /// Gets the Asset Store content link for the current Camera Shake version.
        /// </summary>
        /// <returns>
        /// A relative URL to the Asset Store.
        /// </returns>
        public static string ContentLink()
        {
            return "content/3563";
        }

        static string HumanizeString(string input)
        {
            var sb = new StringBuilder();

            char last = char.MinValue;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsLower(last) && char.IsUpper(c))
                {
                    sb.Append(' ');
                }
                sb.Append(c);
                last = c;
            }
            return sb.ToString();
        }
    }
}

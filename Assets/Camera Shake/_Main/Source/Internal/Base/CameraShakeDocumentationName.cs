//
// CameraShakeDocumentationName.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2014 Thinksquirrel Software, LLC
//
namespace Thinksquirrel.CameraShakeInternal
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    sealed class CameraShakeDocumentationName : System.Attribute
    {
        string m_Name;

        public string name { get { return m_Name; } }

        public CameraShakeDocumentationName(string name)
        {
            this.m_Name = name;
        }
    }
}

/*
AssetValidator 
Copyright (c) 2018 Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEditor.SceneManagement;

namespace JCMG.AssetValidator.Editor.Validators.Output
{
    public class VLog
    {
        public VLogType vLogType;
        public VLogSource source;
        public string validatorName;
        public string message;
        public string scenePath;
        public string objectPath;

        public string GetSourceDescription()
        {
            switch (source)
            {

                case VLogSource.Scene:
                    return scenePath;
                case VLogSource.Project:
                    return "Project";
                default:
                case VLogSource.None:
                    return "None";
            }
        }

        public bool HasObjectPath()
        {
            return !string.IsNullOrEmpty(objectPath);
        }

        public bool CanLoadScene()
        {
            return !string.IsNullOrEmpty(scenePath) &&
                   EditorSceneManager.GetActiveScene().path != scenePath;
        }

        public bool CanPingObject()
        {
            return HasObjectPath() &&
                   (source == VLogSource.Scene && EditorSceneManager.GetActiveScene().path == scenePath ||
                    source == VLogSource.Project);
        }
    }
}

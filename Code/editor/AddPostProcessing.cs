using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VRC.SDKBase;
using Object = System.Object;

namespace akaUdon
{
    public class AddPostProcessing : MonoBehaviour
    {
        [MenuItem("Tools/Settings Menu/Add A Scene Descriptor to your scene",false,51)]
        public static void AddSceneDescriptor()
        {
            if(FindObjectOfType<VRC_SceneDescriptor>() != null) return;
            Object world = AssetDatabase.LoadAssetAtPath("Packages/com.vrchat.worlds/Samples/UdonExampleScene/Prefabs/VRCWorld.prefab", typeof(object));
            
            Object obj = PrefabUtility.InstantiatePrefab(world as GameObject);
            
            Undo.RegisterCreatedObjectUndo((GameObject)obj, $"Undo create");
        }
        
        [MenuItem("Tools/Settings Menu/Add the Post Processing Layer",false,50)]
        public static void AddPostProcessingLayer()
        {
            Camera camera = FindCameraInSceneDescriptor();

            if (camera == null)
            {
                EditorUtility.DisplayDialog("Uh Oh Spaghetti-o", "Something went wrong and the Camera could not be found, make sure one exists in the scene and try again", "ok");
                return;
            }

            GameObject obj = camera.gameObject;
            if (obj.GetComponent<PostProcessLayer>() != null)
            {
                return;
            }

            Undo.AddComponent<PostProcessLayer>(obj);
            var pp = obj.GetComponent<PostProcessLayer>();
            pp.volumeLayer = 16;
            

        }

        private static Camera FindCameraInSceneDescriptor()
        {
            Camera camera = null;
            VRC_SceneDescriptor sceneDescriptor = FindObjectOfType<VRC_SceneDescriptor>();
            
            if (sceneDescriptor != null)
            {
                GameObject g = sceneDescriptor.ReferenceCamera;

                if (g != null)
                {
                    camera = g.GetComponent<Camera>();
                }
                else
                {
                    camera = FindCameraInScene();
                    if (camera == null) return null;
                    sceneDescriptor.ReferenceCamera = camera.gameObject;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Missing VRCWorld", "A VRC Scene Descriptor does not exist yet. Please Add one and try again or " +
                                                                "go to Tools -> Settings Menu -> Add a Scene Descriptor to your Scene", "ok");
                camera = FindCameraInScene();
            }
            
            return camera;
        }

        private static Camera FindCameraInScene()
        {
            return FindObjectOfType<Camera>();
        }
    }
}
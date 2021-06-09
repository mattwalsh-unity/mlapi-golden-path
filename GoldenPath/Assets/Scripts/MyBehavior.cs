using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class MyBehavior : MonoBehaviour {
        // This will store the string value
        [StringInList("Cat", "Dog")]
        public string Animal;
        
        // This will store the index of the array value
        [StringInList("John", "Jack", "Jim")]
        public int PersonID;
    
        // Showing a list of loaded scenes
        [StringInList(typeof(PropertyDrawersHelper), "AllSceneNames")] public string SceneName;
    }
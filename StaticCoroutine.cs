using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoroutine : MonoBehaviour{
 public static StaticCoroutine instance; //the instance of our class that will do the work
 
 void Awake(){ //called when an instance awakes in the game
  instance = this; //set our static reference to our newly initialized instance
 }
 
 IEnumerator PerformCoroutine(){ //the coroutine that runs on our monobehaviour instance
  while(true){
   yield return 0;
  }
 }
 
 static public void DoCoroutine(){
  instance.StartCoroutine("PerformCoroutine"); //this will launch the coroutine on our instance
 }
}
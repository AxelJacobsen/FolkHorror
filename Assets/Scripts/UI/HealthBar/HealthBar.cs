using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Public class implementation for HealthBars
/// </summary>
public class HealthBar : MonoBehaviour
{
  
    public Slider Slider;       // slider for depicting current health
    public Gradient Gradient;   // gradient for changing color of health dependant  
    public Image Fill;          // the image which the slider and gradient apply the colors to
    public GameObject Entity;   // the temp holder for Character Script component loading

    // Loaded character script holder
    private Character entityCharacterScript;
     
    /// <summary>
    /// Sets displayed current health based on inputted value
    /// </summary>
    /// <param name="currHealth">Current health of Character as float</param>
    public void SetCurrHealthVal(float currHealth){
    Slider.value = currHealth;
    Fill.color = Gradient.Evaluate(Slider.normalizedValue);  
    }

    /// <summary>
    /// Sets the maximum value of the slider for displaying health
    /// </summary>
    /// <param name="maxHealth">Maximum health of character</param>
    public void SetMaxHealthVal(float maxHealth){
    Slider.maxValue = maxHealth;
    Slider.value = maxHealth;

    Fill.color = Gradient.Evaluate(1F);
    }

    private void Start(){
        entityCharacterScript = Entity.GetComponent<Character>();
        if (Entity.GetComponent<Character>() == null)       // Debugs if failed to load Character component
            Debug.LogWarning("HealthBarScript could not find the Character Script belonging to the Entity");
        SetMaxHealthVal(entityCharacterScript.MaxHealth);   // Sets maximum slider value to be the same as max health of Character
    }

    private void FixedUpdate(){
        // Sets current health for display
        SetCurrHealthVal(entityCharacterScript.GetCurrentHealth());
    }

}


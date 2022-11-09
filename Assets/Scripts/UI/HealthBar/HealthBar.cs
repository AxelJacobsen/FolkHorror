using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  
    public Slider Slider;
    public Gradient Gradient;
    public Image Fill;
    public GameObject Entity;

    private Character entityCharacterScript;
     
    public void SetCurrHealthVal(float currHealth){
    Slider.value = currHealth;
    Fill.color = Gradient.Evaluate(Slider.normalizedValue);  
    }

    public void SetMaxHealthVal(float maxHealth){
    Slider.maxValue = maxHealth;
    Slider.value = maxHealth;

    Fill.color = Gradient.Evaluate(1F);
    }
    private void Start(){
        entityCharacterScript = Entity.GetComponent<Character>();
        if (Entity.GetComponent<Character>() == null) Debug.LogWarning("HealthBarScript could not find the Character Script belonging to the Entity");
        SetMaxHealthVal(entityCharacterScript.MaxHealth);
    }

    private void FixedUpdate(){
        SetCurrHealthVal(entityCharacterScript.GetCurrentHealth());
    }

}


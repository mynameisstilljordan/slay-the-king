using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Theme : MonoBehaviour{
    [SerializeField] private Material _primaryMaterial, _secondaryMaterial, _particleMaterial;
    
    Color32 red = new Color32(223, 127, 152, 255); //new Color32(255, 77, 77, 255);
    Color32 orange = new Color32(237, 177, 85, 255); //new Color32(255, 153, 51, 255);
    Color32 yellow = new Color32(242, 224, 107, 255); //new Color32(255, 255, 77, 255);
    Color32 green = new Color32(190, 227, 135, 255); //new Color32(77, 255, 77, 255);
    Color32 blue = new Color32(127, 182, 236, 255); //new Color32(77, 136, 255, 255);
    Color32 lightBlue = new Color32(148, 210, 241, 255);
    Color32 skyBlue = new Color32(167, 223, 242, 255);
    Color32 purple = new Color32(155, 135, 236, 255); //new Color32(153, 51, 255, 255);
    Color32 magenta = new Color32(218, 124, 234, 255); //new Color32(234, 0, 255, 255);
    Color32 pink = new Color32(245, 186, 255, 255);

    private Color32[] _allColors = new Color32[] {
        new Color32(246, 255, 117, 255),
        Color.white,
        new Color32(190, 227, 135, 255),
        new Color32(127, 182, 236, 255),
        new Color32(155, 135, 236, 255),
        new Color32(237, 177, 85, 255),
        new Color32(245, 186, 255, 255),
        new Color32(223, 127, 152, 255),
    };
    
    //the requirements for the themes
    private int[] _themeRequirements = new int[] {
        -1, -1, 50, 125, 225, 350, 500, 1000

    };
    
    public Color32 PrimaryColor { get; set; }
    public Color32 SecondaryColor { get; set; }
    
    //load the saved colors
    public void LoadColors() {
        SetPrimaryColor(PlayerPrefs.GetInt("primary",0));
        SetSecondaryColor(PlayerPrefs.GetInt("secondary",1));
    }

    //set the primary color to given index
    public void SetPrimaryColor(int color) {
        PrimaryColor = _allColors[color];
        _primaryMaterial.color = PrimaryColor;
        _particleMaterial.SetColor("_EmissionColor", PrimaryColor);
    }

    //set the secondary color to given index
    public void SetSecondaryColor(int color) {
        SecondaryColor = _allColors[color];
        _secondaryMaterial.color = SecondaryColor;
    }
    
    //save the current colors to memory
    public void SaveColors() {
        PlayerPrefs.SetInt("primary",ConvertColorToIndex(PrimaryColor));
        PlayerPrefs.SetInt("secondary", ConvertColorToIndex(SecondaryColor));
    }

    //convert a color to an index in the all colors array
    public int ConvertColorToIndex(Color32 color) {
        for (int i = 0; i < _allColors.Length; i++) {
            if (_allColors[i].Equals(color)) return i;
        }
        return 0;
    }

    //return the total number of colors
    int NumberOfColors() {
        return _allColors.Length;
    }
    
    //updates the colors of all TMP_Text
    public void UpdateTMPColors() {
        //set all primary text
        var _allPrimaryText = GameObject.FindGameObjectsWithTag("primary");
        foreach (GameObject text in _allPrimaryText) {
            text.GetComponent<TMP_Text>().color = PrimaryColor;
        }
        
        //set all secondary text
        var _allSecondaryText = GameObject.FindGameObjectsWithTag("secondary");
        foreach (GameObject text in _allSecondaryText) {
            text.GetComponent<TMP_Text>().color = SecondaryColor;
        }
    }

    public int GetNextColor(Color32 currentColor) {
        var currentColorIndex = ConvertColorToIndex(currentColor); //convert the current color to the index value and save it
        if (currentColorIndex < _allColors.Length-1) return currentColorIndex + 1; //if not the final index, return the next color
        else return 0; //otherwise, return the first color in the array
    }

    public int GetRequirementAtIndex(int index) {
        return _themeRequirements[index];
    }

    public int GetNextRequirement(int currentKingSlays) {
        for (int i = 0; i < _themeRequirements.Length; i++) {
            if (currentKingSlays < _themeRequirements[i]) return _themeRequirements[i] - currentKingSlays;
        }
        return 0;
    }
}

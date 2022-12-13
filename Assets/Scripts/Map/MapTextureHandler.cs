using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTextureHandler : MonoBehaviour
{
    public List<MapTextures> MaterialProgression;

    [System.Serializable]
    public struct MapTextures {
        public MaterialAlts roof;
        public MaterialAlts floor;
        public MaterialAlts walls;
        //public int specificStage // Don't think this will be used, but could in theory add specific stages to swap style,
        //currently evenly distributed
    }

    [System.Serializable]
    public struct MaterialAlts {
        public Material material;
        public string hexadecimal;
    }

    /// <summary>
    /// Contacts the player and sends local material data to player
    /// </summary>
    /// <param name="player"></param>
    public void TransferMaterialData(Collider player) {
        if (player.transform.parent.tag != "Player" || transform.parent.tag == "Player") {
            //Redundant, but ensures the player doesnt do anything wacky
            return;
        }

        MapTextureHandler playerTextureHolder = player.transform.parent.GetComponent<MapTextureHandler>();
        playerTextureHolder.RecieveMaterialFromPortal(MaterialProgression);
    }

    /// <summary>
    /// Returns three materials depending on where the player is relative to the amount of alternative textures available
    /// </summary>
    /// <param name="currentStage"></param>
    /// <param name="bossStage"></param>
    /// <returns></returns>
    public (Material, Material, Material) RequestMaterialFromPlayer(int currentStage, int bossStage) {
        if (MaterialProgression.Count <= 0) { return (null, null, null); }

        int curTexture = (currentStage * MaterialProgression.Count) / bossStage;
        if (MaterialProgression.Count <= curTexture) {
            curTexture = MaterialProgression.Count - 1;
        }
        //print(curTexture);
        /*//If the player is before the first segment, uses 0, saves on some procesing
        if (pProgress <= matSegments) {
            //Finds correct index depending on player progression
            for (float textThreshold = 1.0f; 0.0f < textThreshold; textThreshold -= matSegments) {
                if (textThreshold < pProgress) {
                    break;
                }
                curTexture++;
                //Preventing wierd bug
                if ((bossStage*2) < curTexture) { print("Material aquisition timed out"); break; }
            }
            //Redundant insurance
            if (MaterialProgression.Count <= curTexture) { curTexture = 0; }
        }*/

        //Construct shaders and set colors


        Shader basic = Shader.Find("Standard");
        Material roofMaterial = GetMaterialFromMaterialAlt(MaterialProgression[curTexture].roof);
        Material floorMaterial = GetMaterialFromMaterialAlt(MaterialProgression[curTexture].floor);
        Material wallMaterial = GetMaterialFromMaterialAlt(MaterialProgression[curTexture].walls);


        //Texture aquisition success
        return (roofMaterial, floorMaterial, wallMaterial);
    }

    /// <summary>
    /// Gets material from MaterialAlt or hexadecimal
    /// </summary>
    /// <param name="inAlt"></param>
    /// <returns></returns>
    private Material GetMaterialFromMaterialAlt(MaterialAlts inAlt) {
        //If We have a material, then use that
        if (inAlt.material != null) { return inAlt.material; }
        //else if we are missing both hexadecimals, and texture, return empty
        else if (string.IsNullOrEmpty(inAlt.hexadecimal)) {
            return null;
        };
        //Enters her if we have hexadecimals
        Color hexColor = new Color();
        ColorUtility.TryParseHtmlString(inAlt.hexadecimal, out hexColor);
        //Failed to parse
        if (hexColor == null) { return null; }

        //Construct shaders and set colors
        Shader basic = Shader.Find("Standard");
        Material outMat = new Material(basic) { color = hexColor };
        return (outMat);
    }

    /// <summary>
    /// This ensures that the player properly recieves textures regardless of unity
    /// </summary>
    /// <param name="inTextures"></param>
    public void RecieveMaterialFromPortal(List<MapTextures> inTextures) {
        MaterialProgression = inTextures;
    }
}

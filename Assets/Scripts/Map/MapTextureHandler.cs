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

        float matSegments = (1.0f / MaterialProgression.Count);
        float pProgress = currentStage / bossStage;
        int curTexture = 0;

        //If the player is before the first segment, uses 0, saves on some procesing
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
        }
        //If all textures are in place, return them
        if (MaterialProgression[curTexture].roof.material != null && 
            MaterialProgression[curTexture].floor.material != null && 
            MaterialProgression[curTexture].walls.material != null) {
            
            return (    MaterialProgression[curTexture].roof.material, 
                        MaterialProgression[curTexture].floor.material, 
                        MaterialProgression[curTexture].walls.material);

            //Otherwise if we are also missing hexadeciamals, then return null

        } else if ( string.IsNullOrEmpty(MaterialProgression[curTexture].roof.hexadecimal) || 
                    string.IsNullOrEmpty(MaterialProgression[curTexture].floor.hexadecimal) || 
                    string.IsNullOrEmpty(MaterialProgression[curTexture].walls.hexadecimal)) {
            return (null, null, null);
        }

        //We will only enter here if there are no textures, but there are hexadecimals
        //ADD CODE TO TRANSLATE HEX INTO MATERIAL

        //Texture equistion failed
        return (null, null, null);
    }
    
    /// <summary>
    /// This ensures that the player properly recieves textures regardless of unity
    /// </summary>
    /// <param name="inTextures"></param>
    public void RecieveMaterialFromPortal(List<MapTextures> inTextures) {
        MaterialProgression = inTextures;
    }
}

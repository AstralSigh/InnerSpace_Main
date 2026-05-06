using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="ScriptableObjects/ConstituentData")]
public class ConstituentData : ScriptableObject
{
    public enum ConstituentType
    {
        Glucose,
        GLUT,
        Pyruvate,
        Mitochondria,
        ATP,
        GLP1,
        GLP1R,
        GProtein,
        GAlpha,
        GDP,
        GTP,
        AC,
        cAMP,
        PKA,
        PKAcd,
        Nucleus,
        mRNA,
        Ribosome,
        ER,
        TransVesicle,
        Golgi,
        InsulinVesicle,
        InsulinMonomer,
        InsulinDimer,
        InsulinHexamer,
        Zinc,
        SNARE,
        Membrane
    }

    public ConstituentType eConType;

    [Header("Graphic Assets")]
    public Sprite conPortrait;
    public Sprite conMetroCardBG;

    [Header("Game Text Entries")]
    public string conName;
    public string pathwayNarrative;
    public List<string> npcDialogs;

    [Header("Bullet Time Data")]
    public string conHeader;
    public string conSubHeader;
    
    public List<string> conInfoPoints;

    [TextArea(5,10)]
    public string conInfoSources;
    public string conPathway;
    public string conPDBNums;

    [TextArea(5, 10)]
    public string conFunction;

    [TextArea(5, 10)]
    public string conDescription;

    [TextArea(5, 10)]
    public string conOrigin;

    [TextArea(5, 10)]
    public string conDimensions; //Volume, Length, Width

    public Material conOpaqueMaterial;

    public Material conClearMaterial;
}

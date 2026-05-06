using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HexamerFormationColorProfiles : ScriptableObject
{
    public List<Material> monomer = new List<Material>();
    public List<Material> dimer = new List<Material>();
    public List<Material> hexamer3 = new List<Material>();
    public List<Material> hexamer2 = new List<Material>();
    public List<Material> hexamer1 = new List<Material>();
    public List<Material> monomerHighlighted = new List<Material>();
    public List<Material> dimerHighlighted = new List<Material>();
    public List<Material> zinc = new List<Material>();
}

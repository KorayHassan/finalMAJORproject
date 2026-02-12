using UnityEngine;

public class Conductor : MonoBehaviour
{
    int crotchetsperber = 8;
    public float bpm = 180;
    public float crotchet;
    public float songposition;
    public float deltasongpos;
    public float lasthit;
    public float actuallasthit;
    private float nextbeattime = 0.0f;
    private float nextbartime = 0.0f;
    public float offset = 0.2f;
    public float addoffset;
    public static float offsetstatic = 0.40f;
    public static bool hasoffsetadjusted = false;
    public int beatnumber = 0;
    public int barnumber = 0;
}

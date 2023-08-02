using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;

[ExecuteInEditMode]
public class MiniMapController : MonoBehaviour
{
    [HideInInspector]
    public Transform shapeColliderGO;
    [HideInInspector]
    public RenderTexture renderTex;
    [HideInInspector]
    public Material mapMaterial;
    [HideInInspector]
    public List<MiniMapEntity> miniMapEntities;
    public GameObject iconPref;
    public GameObject destinationIconPref;
    [HideInInspector]
    public Camera mapCamera;

    public Transform target;

    public LayerMask minimapLayers;
    public bool showBackground;
    public Sprite miniMapMask;
    public Sprite miniMapBorder;
    [Range(0, 1)]
    public float miniMapOpacity = 1;
    public Vector3 miniMapScale = new Vector3(1, 1, 1);

    public Vector3 cameraOffset = new Vector3(0f, 7.5f, 0f);
    public float camSize = 15;
    public float camFarClip = 1000;
    public Vector3 rotationOfCam = new Vector3(90, 0, 0);
    public bool rotateWithTarget = true;
    public Dictionary<GameObject, GameObject> ownerIconMap = new Dictionary<GameObject, GameObject>();

    private GameObject miniMapPanel;
    private Image mapPanelMask;
    private Image mapPanelBorder;
    private Image mapPanel;
    private Color mapColor;
    private Color mapBorderColor;

    private RectTransform mapPanelRect;
    private RectTransform mapPanelMaskRect;

    private Vector3 prevRotOfCam;
    private Vector2 res;
    private Image miniMapPanelImage;

    public void OnEnable()
    {
       

        ownerIconMap.Clear();
        GameObject maskPanelGO = transform.GetComponentInChildren<Mask>().gameObject;
        mapPanelMask = maskPanelGO.GetComponent<Image>();
        mapPanelBorder = maskPanelGO.transform.parent.GetComponent<Image>();
        miniMapPanel = maskPanelGO.transform.GetChild(0).gameObject;
        mapPanel = miniMapPanel.GetComponent<Image>();
        mapColor = mapPanel.color;
        mapBorderColor = mapPanelBorder.color;
        if (mapCamera == null)
            mapCamera = transform.GetComponentInChildren<Camera>();
        mapCamera.cullingMask = minimapLayers;

        mapPanelMaskRect = maskPanelGO.GetComponent<RectTransform>();
        mapPanelRect = miniMapPanel.GetComponent<RectTransform>();
        mapPanelRect.anchoredPosition = mapPanelMaskRect.anchoredPosition;
        res = new Vector2(Screen.width, Screen.height);

        miniMapPanelImage = miniMapPanel.GetComponent<Image>();
        miniMapPanelImage.enabled = !showBackground;
        SetupRenderTexture();
    }

    private void OnDisable()
    {
        if (renderTex != null && !renderTex.IsCreated())
        {
            renderTex.Release();
        }
    }

    private void OnDestroy()
    {
        if (renderTex != null && !renderTex.IsCreated())
        {
            renderTex.Release();
        }
    }

    public void LateUpdate()
    {
        mapPanelMask.sprite = miniMapMask;
        mapPanelBorder.sprite = miniMapBorder;
        mapPanelBorder.rectTransform.localScale = miniMapScale;
        mapBorderColor.a = miniMapOpacity;
        mapColor.a = miniMapOpacity;
        mapPanelBorder.color = mapBorderColor;
        mapPanel.color = mapColor;

        mapPanelMaskRect.sizeDelta = new Vector2(Mathf.RoundToInt(mapPanelMaskRect.sizeDelta.x), Mathf.RoundToInt(mapPanelMaskRect.sizeDelta.y));
        mapPanelRect.position = mapPanelMaskRect.position;
        mapPanelRect.sizeDelta = mapPanelMaskRect.sizeDelta;
        miniMapPanelImage.enabled = !showBackground;

        if (Screen.width != res.x || Screen.height != res.y)
        {
            SetupRenderTexture();
            res.x = Screen.width;
            res.y = Screen.height;
        }

        SetCam();
    }

    private void SetupRenderTexture()
    {
        if (renderTex != null && renderTex.IsCreated())
        {
            renderTex.Release();
        }

        renderTex = new RenderTexture((int)mapPanelRect.sizeDelta.x, (int)mapPanelRect.sizeDelta.y, 24);
        renderTex.Create();

        mapMaterial.mainTexture = renderTex;
        mapCamera.targetTexture = renderTex;

        mapPanelMaskRect.gameObject.SetActive(false);
        mapPanelMaskRect.gameObject.SetActive(true);
    }

    private void SetCam()
    {
        mapCamera.orthographicSize = camSize;
        mapCamera.farClipPlane = camFarClip;

        if (target == null)
        {
            Debug.Log("Please assign the target");
        }
        else
        {
            mapCamera.transform.eulerAngles = rotationOfCam;

            if (rotateWithTarget)
            {
                mapCamera.transform.eulerAngles = target.eulerAngles + rotationOfCam;
            }

            mapCamera.transform.position = target.position + cameraOffset;
        }
    }

    public MapObject RegisterMapObject(GameObject owner, MiniMapEntity mme, bool isDestination = false)
    {
        GameObject curMGO = Instantiate(isDestination ? destinationIconPref : iconPref);
        MapObject curMO = curMGO.AddComponent<MapObject>();
        curMO.SetMiniMapEntityValues(this, mme, owner, mapCamera, miniMapPanel);
        ownerIconMap.Add(owner, curMGO);
        return owner.GetComponent<MapObject>();
    }

    public void UnregisterMapObject(MapObject mmo, GameObject owner)
    {
        if (ownerIconMap.ContainsKey(owner))
        {
            Destroy(ownerIconMap[owner]);
            ownerIconMap.Remove(owner);
        }

        Destroy(mmo);
    }

}

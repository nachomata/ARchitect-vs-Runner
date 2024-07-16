using UnityEngine;
using Vuforia;
using System.Collections.Generic;
using System.Linq;

public class ImageTargetGrid : MonoBehaviour
{
    public static ImageTargetGrid Instance { get; private set; }
    public bool snap = false;
    public string centerTargetName = "key";
    public int gridSize = 21;
    public float spacing = 3.0f;
    public Vector3 startPosition = Vector3.zero;
    private List<ImageTargetBehaviour> allMarkers = new List<ImageTargetBehaviour>();
    private List<ImageTargetBehaviour> TrackingMarkers = new List<ImageTargetBehaviour>();
    private HashSet<string> trackedMarkerNames = new HashSet<string>();
    private List<ImageTargetBehaviour> previousTrackingMarkers = new List<ImageTargetBehaviour>();
    private float positionChangeThreshold = 0.1f;
    private Dictionary<Transform, Vector3> initialOffsets = new Dictionary<Transform, Vector3>();
    public Transform centerTransform;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {        
        if (UpdateMarkerPositions())
        {
            CreateGrid();
        }
    }

    void Update()
    {
        // print(snap);
        if (UpdateMarkerPositions())
        {
            CreateGrid();
        }
    }

    bool UpdateMarkerPositions()
    {
        bool hasChanged = false;
        TrackingMarkers.Clear();
        HashSet<string> currentMarkerNames = new HashSet<string>();
        allMarkers = VuforiaBehaviour.Instance.World.GetObserverBehaviours().OfType<ImageTargetBehaviour>().ToList();

        if (VuforiaBehaviour.Instance == null)
        {
            Debug.LogWarning("VuforiaBehaviour instance is null");
            return false;
        }

        foreach (var marker in allMarkers)
        {
            if (marker is ImageTargetBehaviour &&
                (marker.TargetStatus.Status == Status.TRACKED || marker.TargetStatus.Status == Status.EXTENDED_TRACKED))
            {
                TrackingMarkers.Add(marker);
                currentMarkerNames.Add(marker.TargetName);
            }
        }

        if (!trackedMarkerNames.SetEquals(currentMarkerNames))
        {
            trackedMarkerNames = currentMarkerNames;
            hasChanged = true;
        }
        else
        {
            float totalPositionChange = 0.0f;
            foreach (var markerInfo in TrackingMarkers)
            {
                var previousMarkerInfo = previousTrackingMarkers.FirstOrDefault(m => m.TargetName == markerInfo.TargetName);
                if (previousMarkerInfo.TargetName != null)
                {
                    totalPositionChange += Vector3.Distance(markerInfo.transform.position, previousMarkerInfo.transform.position);
                }
            }

            if (totalPositionChange > positionChangeThreshold)
            {
                hasChanged = true;
            }
        }

        previousTrackingMarkers = new List<ImageTargetBehaviour>(TrackingMarkers);
        return true;
    }

    void CreateGrid()
    {
        var centerMarker = TrackingMarkers.FirstOrDefault(m => m.TargetName == centerTargetName);
        if (centerMarker == null)
        {
            Debug.LogWarning("Center target not found");
        }
        else
        {
            centerTransform = centerMarker.transform;
        }


        // print(centerPosition);

        //float spacing = CalculateAverageSpacing(centerPosition);

        ImageTargetBehaviour[,] grid = new ImageTargetBehaviour[gridSize, gridSize];
        int gridOffset = gridSize / 2;

        grid[gridOffset, gridOffset] = centerMarker;
        foreach (ImageTargetBehaviour marker in TrackingMarkers)
        {
            if (marker!=null){
                if (marker.TargetName != centerMarker.TargetName)
                {
                    Vector3 relativePosition = marker.transform.position - centerTransform.position;
                    int x = Mathf.RoundToInt(relativePosition.x / spacing);
                    int y = Mathf.RoundToInt(relativePosition.y / spacing);

                    if (x >= -gridOffset && x <= gridOffset && y >= -gridOffset && y <= gridOffset)
                    {
                        grid[x + gridOffset, y + gridOffset] = marker;
                    }
                }
            }
        }

        // print(centerTransform);
        if(centerTransform != null)
        {

            Vector3 gridOrigin = centerTransform.position - new Vector3(gridOffset * spacing, gridOffset * spacing, 0);
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    var marker = grid[x, y];
                    if (marker != null)
                    {
                        if (marker.TargetName != null && marker.TargetName != centerTargetName)
                        {
                            Vector3 targetPosition = gridOrigin + new Vector3(x * spacing, y * spacing, 0);
                            foreach (Transform child in GetAllChildObjects(marker.gameObject))
                            {
                                if (!initialOffsets.ContainsKey(child))
                                {
                                    Vector3 offset = child.position - marker.transform.position;
                                    initialOffsets.Add(child, offset);
                                    child.position = offset + targetPosition;
                                    if (snap)
                                    {
                                        child.SetParent(centerTransform);
                                        child.localRotation = new Quaternion(child.localRotation.x, 0, 0, child.localRotation.w);
                                        child.localPosition = new Vector3(child.localPosition.x, centerTransform.position.y, child.localPosition.z);
                                    }
                                }
                                else
                                {
                                    //print(marker.TargetName + child.position);
                                    child.position = initialOffsets[child] + targetPosition;
                                    if (snap)
                                    {
                                        child.SetParent(centerTransform);
                                        child.localRotation = new Quaternion(child.localRotation.x, 0, 0, child.localRotation.w);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }



    List<Transform> GetAllChildObjects(GameObject parent)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in parent.transform)
        {
            if(!child.gameObject.CompareTag("Player"))
                children.Add(child);
        }
        return children;
    }

    float CalculateAverageSpacing(Vector3 centerPosition)
    {
        if (TrackingMarkers.Count <= 1)
            return 3.0f; // default spacing

        float totalDistance = 0.0f;
        int count = 0;

        foreach (var markerInfo in TrackingMarkers)
        {
            if (markerInfo.TargetName != centerTargetName)
            {
                totalDistance += Vector3.Distance(centerPosition, markerInfo.transform.position);
                count++;
            }
        }

        return totalDistance / count;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuild : MonoBehaviour
{
    [SerializeField] GameObject pfWall;
    [SerializeField] GameObject pfWallPlan;
    [SerializeField] private Transform vfxSmoke;
    bool isMakingPlan;
    Vector3 startPosition;
    Vector3 endPosition;
    List<GameObject> plannedWalls = new List<GameObject>();
    List<GameObject> walls = new List<GameObject>();
    const float WALL_LENGTH = 30.0f;
    const float WALL_OFFSET_Y = 7.0f;
    AudioSource soundStones;

    // Start is called before the first frame update
    void Start()
    {
        soundStones = GameObject.Find("/Sound/Stones").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMakingPlan)
        {
            endPosition = GetMousePosition();
            RemoveExistingPlan();
            DrawWalls(pfWallPlan, plannedWalls);
        }
    }

    public void RemoveExistingPlan()
    {
        foreach (GameObject wall in plannedWalls)
        {
            Destroy(wall);
        }
    }

    private int DrawWalls(GameObject pfWall, List<GameObject> wallsList)
    {
        float distanceToGo = (endPosition - startPosition).magnitude;
        Vector3 direction = (endPosition - startPosition).normalized;
        Vector3 currentPosition = startPosition;
        float beginY = Terrain.activeTerrain.SampleHeight(new Vector3(startPosition.x, 500, startPosition.z)) + WALL_OFFSET_Y;
        int numberOfWalls = (int)(distanceToGo / WALL_LENGTH) + 1;
        if (numberOfWalls > Game.Instance.Players[0].Stone * .01)
        {
            numberOfWalls = (int)(Game.Instance.Players[0].Stone * .01);
        }
        for (int i = 0; i < numberOfWalls; i++)
        {
            GameObject newWall = Instantiate(pfWall);
            wallsList.Add(newWall);
            newWall.transform.position = new Vector3(currentPosition.x, beginY, currentPosition.z);
            newWall.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            distanceToGo -= WALL_LENGTH;
            currentPosition += direction * WALL_LENGTH;
            float endY = Terrain.activeTerrain.SampleHeight(new Vector3(currentPosition.x, 500, currentPosition.z)) + WALL_OFFSET_Y;
            newWall.transform.Rotate(180 * Mathf.Atan2(beginY - endY, WALL_LENGTH) / Mathf.PI, 0, 0);
            Debug.Log(beginY + " - " + endY + " - " + 180 * Mathf.Atan2(endY - beginY, WALL_LENGTH) / Mathf.PI);
            beginY = endY;
            if(pfWall==this.pfWall)
            {
                Instantiate(vfxSmoke, newWall.transform.position, Quaternion.identity);
            }
        }

        return numberOfWalls;
    }

    void OnPlaceWall()
    {
        if (!HumanPlayer.Instance.IsBuildingWall)
        {
            return;
        }
        if (!isMakingPlan)
        {
            isMakingPlan = true;
            startPosition = GetMousePosition();
        }
        else
        {
            soundStones.Play();
            HumanPlayer.Instance.ChangeStoneAmount(-100*DrawWalls(pfWall, walls));
            RemoveExistingPlan();
            isMakingPlan = false;
            if (Game.Instance.Players[0].Stone == 0)
            {
                // disable build button
                HumanPlayer.Instance.SetBuildWall(true);
            }
        }
    }

    private Vector3 GetMousePosition()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Terrain");
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            return new Vector3(hit.point.x, 0, hit.point.z);
        }

        return Vector3.zero;
    }

    public void OnCancelBuild()
    {
        HumanPlayer.Instance.SetBuildWall(false);
    }

    public void Reset()
    {
        RemoveExistingPlan();
        isMakingPlan = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPresenter : MonoBehaviour
{
    [SerializeField] Level level;
    [SerializeField] Image roomUIPrefab;
    [SerializeField] float timerToDraw = 0.5f;

    private float localOffsetX = 216;
    private float localOffsetY = 108;

    // Start is called before the first frame update
    void Start()
    {
        roomUIPrefab.transform.position = new Vector3(-783, 0, 0);
    }

    private void OnEnable()
    {
        level.onLevelCreatedAction += UpdateUI;
    }

    private void OnDisable()
    {
        level.onLevelCreatedAction -= UpdateUI;
    }

    private void UpdateUI()
    {
        DrawRooms();
    }

    private void DrawRooms()
    {
        foreach (Room r in level.GetRooms())
        {
            Image currentImage = Instantiate(roomUIPrefab, this.transform);
            currentImage.rectTransform.localPosition += (new Vector3 (localOffsetX, 0, 0) * r.GetRoomNumber());
            new WaitForSeconds(timerToDraw);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

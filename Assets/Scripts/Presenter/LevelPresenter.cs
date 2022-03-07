using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelPresenter : MonoBehaviour
{
    [SerializeField] Level level;
    [SerializeField] GameObject roomUIPrefab;
    [SerializeField] Button resetUIButton;
    [SerializeField] float timerToDraw = 0.5f;

    private int localOffsetX = 188;
    private int localOffsetY = 100;

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
        resetUIButton.interactable = false;
        StartCoroutine(DrawRooms());
    }

    private IEnumerator DrawRooms()
    {
        foreach (Room r in level.GetRooms())
        {
            GameObject roomUI = Instantiate(roomUIPrefab, this.transform);
            Vector2Int imagePos = ((new Vector2Int (localOffsetX, localOffsetY)) * (r.GetPositionInGrid() - level.GetGridOrigin()));
            roomUI.GetComponent<Image>().rectTransform.localPosition += new Vector3 (imagePos.x, imagePos.y, 0);
            if (r.GetRoomType() == 0) roomUI.GetComponent<Image>().color = Color.green;
            if (r.GetRoomType() == 2) roomUI.GetComponent<Image>().color = Color.grey;
            roomUI.GetComponentInChildren<Text>().text = r.GetRoomNumber().ToString();
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                if (r.ContainsDoor(dir)) roomUI.transform.Find("Door "+dir).gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(timerToDraw);
        }
        resetUIButton.interactable = true;
    }

    public void ResetLevel() 
    {
        foreach (Image image in FindObjectsOfType<Image>()) {
            if (image.name.Contains("RoomUI")) Destroy(image.gameObject);
        }
        level.ResetRoomsCreation();
        UpdateUI();
    }

}

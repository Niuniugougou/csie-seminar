﻿using UnityEngine;
using System.Collections;
using OpenCVForUnity;
using UnityEngine.UI;

public class GHSButtonEvent : MonoBehaviour
{

    public GHSPlayerState _playerState;
    public GameObject[] gameTip;
    public GameObject[] playerStateObject;
    public GameObject[] playerCoordinate;
    public GameObject gameStartGround;

    public Button[] _inMenuButton;
    public Button[] _inSettingButton;
    public Button[] _inGameRunButton;
    public Button[] _inListProducerButton;

    public Text _gameStateMenuToSettingButtonText;
    public Text _gameStateSettingToGameRunButtonText;
    public Text _gameStateGameRunToSettingButtonText;
    public Text _gameStateSettingToMenuButtonText;

    private bool _exitFlage = true;
    private bool _resetFlage = false;

    public Text[] _textListProducer;
    public GameObject _backGround;

    void Start()
    {
        GHSPlayerState _playerState = gameObject.GetComponent<GHSPlayerState>();
    }

    void Update()
    {
        this.RunListProducer();
        if (_exitFlage && _resetFlage)
        {
            _backGround.SetActive(false);

            for (int index = 0; index < _inSettingButton.Length; index++)
            {
                _inSettingButton[index].gameObject.SetActive(true);
            }

            RectTransform backGroundRectTransform = _backGround.GetComponent<RectTransform>();
            _textListProducer[0].transform.position = new Vector3(backGroundRectTransform.rect.x, backGroundRectTransform.rect.y - backGroundRectTransform.rect.height, 0);
            _textListProducer[1].transform.position = new Vector3(backGroundRectTransform.rect.x, _textListProducer[0].transform.position.y - backGroundRectTransform.rect.height, 0);
            _textListProducer[2].transform.position = new Vector3(backGroundRectTransform.rect.x, _textListProducer[1].transform.position.y - backGroundRectTransform.rect.height, 0);

            _resetFlage = false;
        }
    }

    //從開始畫面到設定畫面按鈕事件
    public void GameStateMenuToSettingButtonClick()
    {
        for (int index = 0; index < _inMenuButton.Length; index++)
        {
            _inMenuButton[index].gameObject.SetActive(false);
        }

        for (int index = 0; index < _inSettingButton.Length; index++)
        {
            _inSettingButton[index].gameObject.SetActive(true);
        }

        for (int index = 0; index < _inGameRunButton.Length; index++)
        {
            _inGameRunButton[index].gameObject.SetActive(false);
        }

        for (int index = 0; index < _inListProducerButton.Length; index++)
        {
            _inListProducerButton[index].gameObject.SetActive(false);
        }

        _gameStateSettingToGameRunButtonText.text = "開始遊戲";

        gameStartGround.SetActive(true);
    }

    //從設定畫面到遊戲執行按鈕事件
    public void GameStateSettingToGameRunButtonClick()
    {
        for (int index = 0; index < _inMenuButton.Length; index++)
        {
            _inMenuButton[index].gameObject.SetActive(false);
        }

        for (int index = 0; index < _inSettingButton.Length; index++)
        {
            _inSettingButton[index].gameObject.SetActive(false);
        }

        for (int index = 0; index < _inGameRunButton.Length; index++)
        {
            _inGameRunButton[index].gameObject.SetActive(true);
        }

        for (int index = 0; index < _inListProducerButton.Length; index++)
        {
            _inListProducerButton[index].gameObject.SetActive(false);
        }

        _gameStateGameRunToSettingButtonText.text = "設定";

        gameStartGround.SetActive(false);
        ViewActiveEnable();
    }

    //從遊戲執行到設定畫面按鈕事件
    public void GameStateGameRunToSettingButtonClick()
    {
        for (int index = 0; index < _inMenuButton.Length; index++)
        {
            _inMenuButton[index].gameObject.SetActive(false);
        }

        for (int index = 0; index < _inSettingButton.Length; index++)
        {
            _inSettingButton[index].gameObject.SetActive(true);
        }

        for (int index = 0; index < _inGameRunButton.Length; index++)
        {
            _inGameRunButton[index].gameObject.SetActive(false);
        }

        for (int index = 0; index < _inListProducerButton.Length; index++)
        {
            _inListProducerButton[index].gameObject.SetActive(false);
        }

        _gameStateSettingToGameRunButtonText.text = "繼續遊戲";

        gameStartGround.SetActive(true);
    }

    //從設定畫面到開始畫面按鈕事件
    public void GameStateSettingToMenuButtonClick()
    {
        for (int index = 0; index < _inMenuButton.Length; index++)
        {
            _inMenuButton[index].gameObject.SetActive(true);
        }

        for (int index = 0; index < _inSettingButton.Length; index++)
        {
            _inSettingButton[index].gameObject.SetActive(false);
        }

        for (int index = 0; index < _inGameRunButton.Length; index++)
        {
            _inGameRunButton[index].gameObject.SetActive(false);
        }

        for (int index = 0; index < _inListProducerButton.Length; index++)
        {
            _inListProducerButton[index].gameObject.SetActive(false);
        }

        gameStartGround.SetActive(true);
    }

    public void GameStateListProducerButtonClick()
    {

        _exitFlage = false;

        _backGround.SetActive(true);
        for (int index = 0; index < _inListProducerButton.Length; index++)
        {
            _inListProducerButton[index].gameObject.SetActive(true);
        }

        for (int index = 0; index < _inSettingButton.Length; index++)
        {
            _inSettingButton[index].gameObject.SetActive(false);
        }
    }

    public void GameStateListProducerBackToMenuButtonClick()
    {
        _resetFlage = true;
        _exitFlage = true;
        for (int index = 0; index < _inListProducerButton.Length; index++)
        {
            _inListProducerButton[index].gameObject.SetActive(false);
        }

        for (int index = 0; index < _inSettingButton.Length; index++)
        {
            _inSettingButton[index].gameObject.SetActive(true);
        }
    }

    void RunListProducer()
    {
        if (_textListProducer[2].transform.position.y >= 500)
        {
            _resetFlage = true;
        }

        if (_textListProducer[2].transform.position.y < 500 && !_exitFlage)//_textListProducer[2].transform.position.y < 300 ||
        {
            _textListProducer[0].transform.Translate(0, 10, 0);
            _textListProducer[1].transform.Translate(0, 10, 0);
            _textListProducer[2].transform.Translate(0, 10, 0);
        }
        else
        {
            _exitFlage = true;
            for (int index = 0; index < _inListProducerButton.Length; index++)
            {
                _inListProducerButton[index].gameObject.SetActive(false);
            }
        }
    }

    private void ViewActiveEnable()
    {
        //遊戲提示顯示
        for (int i = 0; i < gameTip.Length; i++)
        {
            if (gameTip[i] == null) continue;
            gameTip[i].SetActive(!gameTip[i].activeSelf);
        }

        //玩家狀態顯示
        for (int i = 0; i < playerStateObject.Length; i++)
        {
            if (playerStateObject[i] == null || !_playerState.GetIsPlayerEnableOrNotByIndex(i)) continue;
            playerStateObject[i].SetActive(!playerStateObject[i].activeSelf);
        }

        //玩家座標顯示
        for (int i = 0; i < playerCoordinate.Length; i++)
        {
            if (playerCoordinate[i] == null || !_playerState.GetIsPlayerEnableOrNotByIndex(i)) continue;
            playerCoordinate[i].SetActive(!playerCoordinate[i].activeSelf);
        }
    }
}

﻿using System.IO;
using MicropolisCore;
using MicropolisEngine;
using UnityEngine;
using UnityEngine.UI;

namespace MicropolisGame
{
    public class GameManager : MonoBehaviour
    {
        private MicropolisUnityEngine _engine;
        private TileManager _tileManager;
        private CallbackManager _callbacks;
        private UIManager _uiManager;

        public short simSpeed;

        [HideInInspector]
        public UIManager UIManager { get { return _uiManager; } }

        private void Start()
        {
            _uiManager = FindObjectOfType<UIManager>();
            _engine = MicropolisUnityEngine.CreateUnityEngine();
            _tileManager = new TileManager(_engine);
            _callbacks = new CallbackManager(this, _engine);
        }

        private void Update()
        {
            // TODO probably should have a separate class to handle general input?
            if (Input.GetKeyDown(KeyCode.Escape) && !_engine.simPaused)
            {
                Pause();
            }

            var ellapsedMilliseconds = (int) (Time.deltaTime * 1000);
            if (ellapsedMilliseconds % 16 == 0)
            {
                // TODO should the check for speed be outside of this if?
                if (_engine.simSpeed != 0)
                {
                    _engine.tickEngine();
                    _tileManager.Draw();
                }
            }
        }

        public void GenerateRandomCity()
        {
            _uiManager.HideMainPanel();
            _engine.initGame();
            _engine.simInit();
            _engine.generateMap();
            _engine.setSpeed(simSpeed);
            _engine.doSimInit();
            _engine.setEnableDisasters(false);
            _uiManager.ToggleGameElements(true);
        }

        public void LoadCity()
        {
            _uiManager.HideMainPanel();
            _engine.initGame();
            _engine.simInit();
            _engine.loadCity("cities" + Path.DirectorySeparatorChar + "haight.cty");
            _engine.setSpeed(simSpeed);
            _engine.doSimInit();
            _engine.setEnableDisasters(false);
            _uiManager.ToggleGameElements(true);
        }

        public void LoadScenario()
        {
            _uiManager.HideMainPanel();
            _engine.loadScenario(ScenarioType.SC_DULLSVILLE);
            _uiManager.ToggleGameElements(true);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Pause()
        {
            _engine.pause();
            _uiManager.ShowPausePanel();
        }

        public void QuitToMain()
        {
            _uiManager.HidePausePanel();
            // TODO end the simulator
            _uiManager.ShowMainPanel();
        }

        public void Resume()
        {
            _uiManager.HidePausePanel();
            _engine.resume();
        }

        public void LoadGraphics()
        {
            // TODO show a list of graphic tilesets
            // TODO let the user pick from the list of tilesets
            var tilesets = new[]
            {
                "x11",
                "classic",
                "classic95",
                "wildwest",
                "mooncolony",
                "medievaltimes",
                "futureusa",
                "futureeurope",
                "ancientasia"
            };
            // for now just randomly pick a tileset
            _tileManager.Engine.LoadTileset(tilesets[Random.Range(0, tilesets.Length)]);
            Resume();
        }

        public void SetSpeed(Dropdown dropdown)
        {
            Debug.Log("dropdown: " + dropdown.itemText + " value: " + dropdown.value);
            var speed = dropdown.value;
            _engine.setSpeed((short) speed);
        }
    }
}
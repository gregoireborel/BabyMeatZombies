using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tile_Engine
{
    /*Enum for menu*/
    enum eMenuButton
    {
        NEW_GAME,
        LOAD,
        OPTION,
        EXIT,
        CREDIT,
        RETURN,
        SOUND,
        MUSIC,
        YES,
        NO,
        SAVE
    };

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /* XNA */
        GraphicsDeviceManager _graphics;                            // GraphicsDeviceManager
        SpriteBatch _spriteBatch;                                   // SpriteBatch
        int _width = 800, _height = 600;                            // Window size

        /* Map */
        Level _level;                                               // Current level
        int _lvlNb;                                                 // Number's current elvel
        int _lvlMax;                                                // Level to reach to win

        /* Joueur */
        Player _player = new Player();                              // Player
        AnimationSprite[] _playerSprite = new AnimationSprite[5];   // Player's sprites
        Rectangle[] _playerSrcRecSprite = new Rectangle[5];         // Recherche de la bonne partie du sprite du joueur

        /* Zombies */
        ZombiesManager _zombieManager;                              // Manager de Zombies

        /* Clef */
        Texture2D _key;                                             // Keys' textures

        /* Portes */
        Texture2D _door;                                            // Doors' textures

        /* Gestion des events */
        private bool _inGame;                                       // If in game or not
        private bool _inPause;                                      // If paused or not
        private bool _gameOver;                                     // If player died
        private bool _clicked;                                      // If clicked or not
        private bool _pushed;                                       // If pressed or not
        private bool _endlevel;                                     // If end of level
        private bool _load;                                         // If load a savegame
        private bool _win;                                          // If we won
        private bool _optionMenu;                                   // If in option menu
        private bool _creditPage;                                   // If watching credits
        private MouseState _mouseState;                             // Mouse management
        private KeyboardState _kbState;                             // Keyboard management

        /* Menu */
        Texture2D _bg;                                              // Main menu background
        Texture2D _gameOverBg;                                      // Game over background
        Texture2D _creditsBg;                                       // Credits background
        Texture2D _victoryBg;                                       // Victory background
        MenuButton[] _menuButton = new MenuButton[11];              // Menu buttons

        /* Light */
        Texture2D _fog;                                             // Fog
        Texture2D[] _torch = new Texture2D[4];                      // Torch

        /* Save && Load */
        Save _save = new Save();                                    // Save

        /* Sound */
        private bool _music;                                        // Background music
        private bool _musicPlaying;                                 // Musique deja en train de tourner?
        private bool _soundEffect;                                  // Sounds
        private Song song;                                          // Objet pour gerer la musique de fond
        SoundEffect[] _growlTab = new SoundEffect[4];    
        private SoundEffect doorOpening;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            #region XNA
            this._graphics.IsFullScreen = false;
            this._graphics.PreferredBackBufferWidth = _width;
            this._graphics.PreferredBackBufferHeight = _height;
            this._graphics.ApplyChanges();

            this.Window.Title = "PFA";
            this.Window.AllowUserResizing = false;
            this.IsMouseVisible = true;
            #endregion

            #region BOOLEAN_AND_GAME_DATA
            /* Initialise les boolean */
            _inGame = false;
            _clicked = false;
            _inPause = false;
            _gameOver = false;
            _pushed = false;
            _endlevel = true;
            _load = false;
            _win = false;
            _music = true;
            _musicPlaying = false;
            _soundEffect = true;
            _optionMenu = false;
            _creditPage = false;

            /* On set le level par defaut */
            _lvlNb = 1;
            _lvlMax = 3;
            #endregion

            #region PLAYER_SPRITE
            this._playerSprite[0] = new AnimationSprite(this, new AnimationDefinition()
            {
                AssetName = "Player/Animations/Walk/baby_down",
                FrameRate = 10,
                FrameSize = new Point(22, 25),
                Loop = true,
                NbFrames = new Point(4, 1)
            });
            _playerSrcRecSprite[0] = new Rectangle(0, 25, 22, 25);
            this._playerSprite[1] = new AnimationSprite(this, new AnimationDefinition()
            {
                AssetName = "Player/Animations/Walk/baby_left",
                FrameRate = 10,
                FrameSize = new Point(22, 25),
                Loop = true,
                NbFrames = new Point(4, 1)
            });
            _playerSrcRecSprite[1] = new Rectangle(0, 50, 22, 25);
            this._playerSprite[2] = new AnimationSprite(this, new AnimationDefinition()
            {
                AssetName = "Player/Animations/Walk/baby_up",
                FrameRate = 10,
                FrameSize = new Point(22, 25),
                Loop = true,
                NbFrames = new Point(4, 1)
            });
            _playerSrcRecSprite[2] = new Rectangle(0, 0, 22, 25);
            this._playerSprite[3] = new AnimationSprite(this, new AnimationDefinition()
            {
                AssetName = "Player/Animations/Walk/baby_right",
                FrameRate = 10,
                FrameSize = new Point(22, 25),
                Loop = true,
                NbFrames = new Point(4, 1)
            });
            _playerSrcRecSprite[3] = new Rectangle(0, 75, 22, 25);
            this._playerSprite[4] = new AnimationSprite(this, new AnimationDefinition()
            {
                AssetName = "Player/Animations/dieing",
                FrameRate = 10,
                FrameSize = new Point(30, 40),
                Loop = true,
                NbFrames = new Point(8, 1)
            });
            _playerSrcRecSprite[4] = new Rectangle(0, 30, 30, 40);

            for (int i = 0; i < 5; i++)
                this._playerSprite[i].Initialize();
            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            #region PLAYER
            for (int i = 0; i < 4; i++)
                this._playerSprite[i].LoadContent(_spriteBatch);
            _player._fullPlayerSprite = Content.Load<Texture2D>("Player/Static/baby_static");
            #endregion

            #region LOAD_LIGHT
            // load fog
            _fog = Content.Load<Texture2D>("fog");

            /* Loading of the torch */
            _torch[0] = Content.Load<Texture2D>("Torche/bas");
            _torch[1] = Content.Load<Texture2D>("Torche/gauche");
            _torch[2] = Content.Load<Texture2D>("Torche/haut");
            _torch[3] = Content.Load<Texture2D>("Torche/droite");
            #endregion

            #region MENU_BUTTON
            /* Chargement des images du menu */
            _menuButton[(int)eMenuButton.NEW_GAME] = new MenuButton(Content.Load<Texture2D>("Boutons/nouvelle-partie"),
                                Content.Load<Texture2D>("Boutons/nouvelle-partie"),
                                200, 50);
            _menuButton[(int)eMenuButton.NEW_GAME]._position = new Vector2(183, 170);
            _menuButton[(int)eMenuButton.LOAD] = new MenuButton(Content.Load<Texture2D>("Boutons/charger"),
                                Content.Load<Texture2D>("Boutons/charger"),
                                200, 50);
            _menuButton[(int)eMenuButton.LOAD]._position = new Vector2(183, 230);
            _menuButton[(int)eMenuButton.OPTION] = new MenuButton(Content.Load<Texture2D>("Boutons/options"),
                                Content.Load<Texture2D>("Boutons/options"),
                                200, 50);
            _menuButton[(int)eMenuButton.OPTION]._position = new Vector2(183, 290);

            _menuButton[(int)eMenuButton.CREDIT] = new MenuButton(Content.Load<Texture2D>("Boutons/credits"),
                                Content.Load<Texture2D>("Boutons/credits"),
                                200, 50);
            _menuButton[(int)eMenuButton.CREDIT]._position = new Vector2(183, 350);
            _menuButton[(int)eMenuButton.EXIT] = new MenuButton(Content.Load<Texture2D>("Boutons/quitter"),
                    Content.Load<Texture2D>("Boutons/quitter"),
                    200, 50);
            _menuButton[(int)eMenuButton.EXIT]._position = new Vector2(183, 410);
            _menuButton[(int)eMenuButton.RETURN] = new MenuButton(Content.Load<Texture2D>("Boutons/retour"),
                                Content.Load<Texture2D>("Boutons/retour"),
                                200, 50);
            _menuButton[(int)eMenuButton.RETURN]._position = new Vector2(183, 410);
            _menuButton[(int)eMenuButton.SOUND] = new MenuButton(Content.Load<Texture2D>("Boutons/son"),
                                Content.Load<Texture2D>("Boutons/son"),
                                200, 50);
            _menuButton[(int)eMenuButton.SOUND]._position = new Vector2(183, 230);
            _menuButton[(int)eMenuButton.MUSIC] = new MenuButton(Content.Load<Texture2D>("Boutons/musique"),
                                Content.Load<Texture2D>("Boutons/musique"),
                                200, 50);
            _menuButton[(int)eMenuButton.MUSIC]._position = new Vector2(183, 170);
            _menuButton[(int)eMenuButton.YES] = new MenuButton(Content.Load<Texture2D>("Boutons/oui"),
                                Content.Load<Texture2D>("Boutons/oui"),
                                200, 50);
            _menuButton[(int)eMenuButton.YES]._position = new Vector2(0, 0);
            _menuButton[(int)eMenuButton.NO] = new MenuButton(Content.Load<Texture2D>("Boutons/non"),
                                Content.Load<Texture2D>("Boutons/non"),
                                200, 50);
            _menuButton[(int)eMenuButton.NO]._position = new Vector2(0, 0);
            _menuButton[(int)eMenuButton.SAVE] = new MenuButton(Content.Load<Texture2D>("Boutons/sauvegarder"),
                                Content.Load<Texture2D>("Boutons/sauvegarder"),
                                200, 50);
            _menuButton[(int)eMenuButton.SAVE]._position = new Vector2(300, 230);
            #endregion

            song = Content.Load<Song>("Songs/mountain_king");
            _growlTab[0] = Content.Load<SoundEffect>("Songs/growl1");
            _growlTab[1] = Content.Load<SoundEffect>("Songs/growl2");
            _growlTab[2] = Content.Load<SoundEffect>("Songs/growl3");
            _growlTab[3] = Content.Load<SoundEffect>("Songs/death");
            doorOpening = Content.Load<SoundEffect>("Songs/doorOpening");

            _bg = Content.Load<Texture2D>("background");
            _gameOverBg = Content.Load<Texture2D>("game-over");
            _creditsBg = Content.Load<Texture2D>("credits");
            _victoryBg = Content.Load<Texture2D>("victory");
            _key = Content.Load<Texture2D>("key");
            _door = Content.Load<Texture2D>("door");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            song.Dispose();
            _growlTab[0].Dispose();
            _growlTab[1].Dispose();
            _growlTab[2].Dispose();
            _growlTab[3].Dispose();
            doorOpening.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            #region DATA_GLOBAL
            /* Initialise les events */
            _kbState = Keyboard.GetState();
            _mouseState = Mouse.GetState();

            /* Affiche la souris */
            this.IsMouseVisible = true;

            /* Quitte le jeu si on clique sur la petite croix */
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (!_player.isAlive())
                _gameOver = true;

            /* Si on a atteint le lvl max */
            if (_lvlNb == _lvlMax)
                _win = true;
            #endregion

            if (_music && MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(song);
            if (!_music && MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Stop();

            if (_inGame) /* EN JEU */
            {
                if (_inPause == false && !_gameOver && !_win)   /* If game is not paused */
                {
                    #region JEU

                    /* On efface la souris */
                    this.IsMouseVisible = false;

                    /* Chargement nouveau niveau */
                    if (_endlevel)
                    {
                        if (_load)
                        {
                            #region LOAD_SAVE

                            if (_save.load())
                            {
                                /* We load the level */
                                _level = new Level(_save._lvl, Content);

                                /* On remplace les infos de collideMap, clefs, portes dans level */
                                _level._colideMap = _save._colideMap;
                                _level._keyTab = _save._keyTab;
                                _level._doorTab = _save._doorTab;
                                _level._nbKey = _save._keyLvl;
                                _level._nbKeyDraw = _save._keyLvl;
                                _level._nbDoor = _save._keyLvl + _save._nbKey;
                                _level._nbDoorDraw = _save._keyLvl + _save._nbKey;

                                /* On changes les coordonnees posX/Y et posX/Ypix dans le joueur */
                                _player._posX = _save._posX;
                                _player._posY = _save._posY;
                                _player._posXpix = _player._posX * 16;
                                _player._posYpix = _player._posY * 16;

                                /* On change les infos du nombre de clef dans le joueur */
                                _player._nbKeys = _save._nbKey;

                                /* On charge le zombiesManager */
                                _zombieManager = new ZombiesManager(ref _level, this, ref _spriteBatch, Content, ref _growlTab, ref _player);

                                /* On change toutes les posX/Y de chaque zombies dans le zombiesManager */
                                for (int i = 0; i < _save._nbZombies; ++i)
                                {
                                    _zombieManager._zombieTab[i]._posX = _save._posZombies[i][0];
                                    _zombieManager._zombieTab[i]._posY = _save._posZombies[i][1];
                                }

                            }
                            else
                            {
                                _inGame = false;
                                _load = false;
                                return;
                            }
                            _load = false;

                            #endregion
                        }
                        else
                        {
                            #region LOAD_LEVEL

                            /* On charge le level */
                            _level = new Level(_lvlNb, Content);

                            /* On initialise les infos du joueur */
                            _player._posX = (int)_level._spawn.X;
                            _player._posY = (int)_level._spawn.Y;
                            _player._posXpix = _player._posX * 16;
                            _player._posYpix = _player._posY * 16;

                            /* On initialise les infos des zombies */
                            _zombieManager = new ZombiesManager(ref _level, this, ref _spriteBatch, Content, ref _growlTab, ref _player);

                            #endregion
                        }
                        _endlevel = false;
                    }

                    /*Convertit la position en pixel en position sur la map*/
                    _player._posX = _player._posXpix / 16;
                    _player._posY = _player._posYpix / 16;

                    #region CLAVIER
                    _player._walk = (_kbState.IsKeyDown(Keys.Up) || _kbState.IsKeyDown(Keys.Down) || _kbState.IsKeyDown(Keys.Left) || _kbState.IsKeyDown(Keys.Right));

                    if (_kbState.IsKeyDown(Keys.Up))
                    {
                        if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix - 1) / 16][_player._posX] == 5)
                            checkDoor((_player._posYpix - 1) / 16, _player._posX, 5);
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix - 1) / 16][_player._posX] == 6)
                            checkDoor((_player._posYpix - 1) / 16, _player._posX, 6);
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix - 1) / 16][_player._posX] == 7)
                            checkDoor((_player._posYpix - 1) / 16, _player._posX, 7);
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix - 1) / 16][_player._posX] == 8)
                            checkDoor((_player._posYpix - 1) / 16, _player._posX, 8);
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix - 1) / 16][_player._posX] == 3)
                            _gameOver = true;
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix - 1) / 16][_player._posX] == 9)
                        {
                            _endlevel = true;
                            _lvlNb++;
                        }
                        _player._posYpix -= checkCollision(2, (_player._posYpix > 16 && ((_level._colideMap[(_player._posYpix - 1) / 16][_player._posX] == 0) || (_level._colideMap[(_player._posYpix - 1) / 16][_player._posX] == 4))));
                    }
                    else if (_kbState.IsKeyDown(Keys.Down))
                    {
                        if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix + 1) / 16][_player._posX] == 5)
                            checkDoor((_player._posYpix + 1) / 16, _player._posX, 5);
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix + 1) / 16][_player._posX] == 6)
                            checkDoor((_player._posYpix + 1) / 16, _player._posX, 6);
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix + 1) / 16][_player._posX] == 7)
                            checkDoor((_player._posYpix + 1) / 16, _player._posX, 7);
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix + 1) / 16][_player._posX] == 8)
                            checkDoor((_player._posYpix + 1) / 16, _player._posX, 8);
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix + 1) / 16][_player._posX] == 3)
                            _gameOver = true;
                        else if (_player._posYpix > 16 && _level._colideMap[(_player._posYpix + 1) / 16][_player._posX] == 9)
                        {
                            _endlevel = true;
                            _lvlNb++;
                        }
                        _player._posYpix += checkCollision(0, (_player._posYpix < _height && ((_level._colideMap[(_player._posYpix + 1) / 16][_player._posX] == 0) || (_level._colideMap[(_player._posYpix + 1) / 16][_player._posX] == 4))));
                    }
                    else if (_kbState.IsKeyDown(Keys.Left))
                    {
                        if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix - 1) / 16] == 5)
                            checkDoor(_player._posY, (_player._posXpix - 1) / 16, 5);
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix - 1) / 16] == 6)
                            checkDoor(_player._posY, (_player._posXpix - 1) / 16, 6);
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix - 1) / 16] == 7)
                            checkDoor(_player._posY, (_player._posXpix - 1) / 16, 7);
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix - 1) / 16] == 8)
                            checkDoor(_player._posY, (_player._posXpix - 1) / 16, 8);
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix - 1) / 16] == 3)
                            _gameOver = true;
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix - 1) / 16] == 9)
                        {
                            _endlevel = true;
                            _lvlNb++;
                        }
                        _player._posXpix -= checkCollision(1, (_player._posXpix > 16 && ((_level._colideMap[_player._posY][(_player._posXpix - 1) / 16] == 0) || (_level._colideMap[_player._posY][(_player._posXpix - 1) / 16] == 4))));
                    }
                    else if (_kbState.IsKeyDown(Keys.Right))
                    {
                        if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix + 1) / 16] == 5)
                            checkDoor(_player._posY, (_player._posXpix + 1) / 16, 5);
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix + 1) / 16] == 6)
                            checkDoor(_player._posY, (_player._posXpix + 1) / 16, 6);
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix + 1) / 16] == 7)
                            checkDoor(_player._posY, (_player._posXpix + 1) / 16, 7);
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix + 1) / 16] == 8)
                            checkDoor(_player._posY, (_player._posXpix + 1) / 16, 8);
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix + 1) / 16] == 3)
                            _gameOver = true;
                        else if (_player._posYpix > 16 && _level._colideMap[_player._posY][(_player._posXpix + 1) / 16] == 9)
                        {
                            _endlevel = true;
                            _lvlNb++;
                        }
                        _player._posXpix += checkCollision(3, (_player._posXpix < _width && ((_level._colideMap[_player._posY][(_player._posXpix + 1) / 16] == 0) || (_level._colideMap[_player._posY][(_player._posXpix + 1) / 16] == 4))));
                    }
                 /*   else if (_kbState.IsKeyDown(Keys.Space))
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            for (int y = 0; y < 20; y++)
                                Debug.Write(_level._colideMap[i][y] + " ");
                            Debug.Write("\n");
                        }
                        Debug.Write("posXpix = " + _player._posXpix + " - posYpix = " + _player._posYpix);
                    }*/
                    else if (_kbState.IsKeyDown(Keys.Escape) && _pushed == false)
                    {
                        _pushed = true;
                        _inPause = true;
                    }

                    /* Si touche relache */
                    if (_kbState.IsKeyUp(Keys.Escape) && _pushed)
                        _pushed = false;
                    #endregion

                    /* Si le joueur marche */
                    if (_player.isWalking())
                        this._playerSprite[_player._direction].Update(gameTime);
                    else
                        _level._colideMap[_player._posY][_player._posX] = 2;

                    /* Update Zombie Manager */
                    _zombieManager.update(ref _level, ref _player, gameTime);

                    #endregion
                }
                else if (_gameOver)                             /* If game over */
                {
                    /* Mouse */
                    #region SOURIS
                    /* Bouton Retour */
                    if (_menuButton[(int)eMenuButton.NEW_GAME].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                    {
                        _clicked = true;
                        _gameOver = false;
                        _endlevel = true;
                        _player.newGame();
                    }

                    /* Bouton Quitter */
                    if (_menuButton[(int)eMenuButton.EXIT].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                    {
                        _clicked = true;
                        _inGame = false;
                        _gameOver = false;
                        _player.newGame();/* A LAISSER!! (Sinon le bouton quitter sur l'ecran game over fonctionne pas)*/
                    }

                    /* Si le clic est relache */
                    if (_mouseState.LeftButton == ButtonState.Released)
                        _clicked = false;
                    #endregion

                    /* Keyboard */
                    #region CLAVIER

                    /* Touche Echap */
                    if (_kbState.IsKeyDown(Keys.Escape) && _pushed == false)
                    {
                        _pushed = true;
                        _inPause = false;
                    }

                    /* Si touche relache */
                    if (_kbState.IsKeyUp(Keys.Escape) && _pushed)
                        _pushed = false;

                    #endregion
                }
                else if (_optionMenu)
                {
                    /* Mouse */
                    #region SOURIS
                    if (_menuButton[(int)eMenuButton.MUSIC].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                        && _clicked == false)
                    {
                        _clicked = true;
                        _music = _music ? false : true;
                    }

                    if (_menuButton[(int)eMenuButton.SOUND].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                        && _clicked == false)
                    {
                        _clicked = true;
                        _soundEffect = _soundEffect ? false : true;
                        _zombieManager._soundEffect = _soundEffect;
                    }

                    /* Bouton return */
                    if (_menuButton[(int)eMenuButton.RETURN].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                    {
                        _clicked = true;
                        _optionMenu = false;
                    }

                    /* Si le clic est relache */
                    if (_mouseState.LeftButton == ButtonState.Released)
                        _clicked = false;

                    #endregion

                    /* Keyboard */
                    #region CLAVIER

                    /* Touche Echap */
                    if (_kbState.IsKeyDown(Keys.Escape) && _pushed == false)
                    {
                        _pushed = true;
                        _inPause = false;
                    }

                    /* Si touche relache */
                    if (_kbState.IsKeyUp(Keys.Escape) && _pushed)
                        _pushed = false;

                    #endregion

                }
                else if (_inPause)                              /* If game is paused */
                {
                    /* Mouse */
                    #region SOURIS
                    /* Bouton Retour */
                    if (_menuButton[(int)eMenuButton.RETURN].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                    {
                        _clicked = true;
                        _inPause = false;
                    }

                    /* Bouton Quitter */
                    if (_menuButton[(int)eMenuButton.EXIT].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                    {
                        _clicked = true;
                        _inPause = false;
                        _inGame = false;
                    }

                    /* Bouton Save */
                    if (_menuButton[(int)eMenuButton.SAVE].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                    {
                        _save.save(ref _level, ref _zombieManager, ref _player);
                        _clicked = true;
                        _inPause = false;
                    }

                    /* Bouton Option */
                    if (_menuButton[(int)eMenuButton.OPTION].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                    {
                        _clicked = true;
                        _optionMenu = true;
                    }

                    /* Si le clic est relache */
                    if (_mouseState.LeftButton == ButtonState.Released)
                        _clicked = false;
                    #endregion

                    /* Keyboard */
                    #region CLAVIER

                    /* Touche Echap */
                    if (_kbState.IsKeyDown(Keys.Escape) && _pushed == false)
                    {
                        _pushed = true;
                        _inPause = false;
                    }

                    /* Si touche relache */
                    if (_kbState.IsKeyUp(Keys.Escape) && _pushed)
                        _pushed = false;

                    #endregion
                }
                else if (_win)                                  /* If we won */
                {
                    /* Mouse */
                    #region SOURIS
                    /* Bouton Retour */
                    if (_menuButton[(int)eMenuButton.NEW_GAME].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                    {
                        _clicked = true;
                        _win = false;
                        _endlevel = true;
                        _lvlNb = 1;
                        _player.newGame();
                    }

                    /* Bouton Quitter */
                    if (_menuButton[(int)eMenuButton.EXIT].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                    {
                        _clicked = true;
                        _inGame = false;
                        _lvlNb = 1;
                        _win = false;
                    }

                    /* Si le clic est relache */
                    if (_mouseState.LeftButton == ButtonState.Released)
                        _clicked = false;
                    #endregion

                    /* Keyboard */
                    #region CLAVIER

                    /* Touche Echap */
                    if (_kbState.IsKeyDown(Keys.Escape) && _pushed == false)
                    {
                        _pushed = true;
                        _inPause = false;
                    }

                    /* Si touche relache */
                    if (_kbState.IsKeyUp(Keys.Escape) && _pushed)
                        _pushed = false;

                    #endregion
                }
            }
            else if (_optionMenu)
            {
                /* Souris */
                #region SOURIS
                if (_menuButton[(int)eMenuButton.MUSIC].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                {
                    _clicked = true;
                    _music = _music ? false : true;
                }

                /* Bouton return */
                if (_menuButton[(int)eMenuButton.RETURN].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                && _clicked == false)
                {
                    _clicked = true;
                    _optionMenu = false;
                }

                if (_menuButton[(int)eMenuButton.SOUND].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                {
                    _clicked = true;
                    _soundEffect = _soundEffect ? false : true;
                }

                /* Si le clic est relache */
                if (_mouseState.LeftButton == ButtonState.Released)
                    _clicked = false;

                #endregion

                /* Clavier */
                #region CLAVIER

                /* Touche Echap */
                if (_kbState.IsKeyDown(Keys.Escape) && _pushed == false)
                {
                    _pushed = true;
                    _inPause = false;
                }

                /* Si touche relache */
                if (_kbState.IsKeyUp(Keys.Escape) && _pushed)
                    _pushed = false;

                #endregion
            
            }
            else if (_creditPage)
            {
                /* Souris */
                #region SOURIS
                /* Bouton return */
                if (_menuButton[(int)eMenuButton.RETURN].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                && _clicked == false)
                {
                    _clicked = true;
                    _creditPage = false;
                }

                /* Si le clic est relache */
                if (_mouseState.LeftButton == ButtonState.Released)
                    _clicked = false;

                #endregion

                /* Clavier */
                #region CLAVIER

                /* Touche Echap */
                if (_kbState.IsKeyDown(Keys.Escape) && _pushed == false)
                {
                    _pushed = true;
                    _inPause = false;
                }

                /* Si touche relache */
                if (_kbState.IsKeyUp(Keys.Escape) && _pushed)
                    _pushed = false;

                #endregion

            }
            else /* MENU DE BASE */
            {
                /* Mouse */
                #region SOURIS
                /* Bouton Nouveau Jeu */
                if (_menuButton[(int)eMenuButton.NEW_GAME].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                {
                    _clicked = true;
                    _inGame = true;
                    _lvlNb = 1;
                    _endlevel = true;
                    _player.newGame();
                }

                /* Bouton Load */
                if (_menuButton[(int)eMenuButton.LOAD].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                {
                    _clicked = true;
                    _inGame = true;
                    _endlevel = true;
                    _load = true;
                }

                /* Bouton options */
                if (_menuButton[(int)eMenuButton.OPTION].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                {
                    _clicked = true;
                    _optionMenu = true;
                }

                /* Bouton options */
                if (_menuButton[(int)eMenuButton.CREDIT].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                    && _clicked == false)
                {
                    _clicked = true;
                    _creditPage = true;
                }

                /* Bouton Quitter */
                if (_menuButton[(int)eMenuButton.EXIT].Update(_mouseState.X, _mouseState.Y) && (_mouseState.LeftButton == ButtonState.Pressed)
                && _clicked == false)
                    this.Exit();

                /* Si le clic est relache */
                if (_mouseState.LeftButton == ButtonState.Released)
                    _clicked = false;
                #endregion

                /* Keyboard */
                #region CLAVIER
                if (_kbState.IsKeyDown(Keys.Escape))
                    this.Exit();
                #endregion
            }
            base.Update(gameTime);
        }

        protected int checkCollision(int new_direction, bool condition)
        {
            _player._direction = new_direction;
            if (_level._colideMap[_player._posY][_player._posX] == 4)
                _player.getKey(_player._posX, _player._posY, _level);
            else if (_level._colideMap[_player._posY][_player._posX] == 3)
            {
                _growlTab[3].Play();
                _gameOver = true;
            }
            _level._colideMap[_player._posYpix / 16][_player._posXpix / 16] = 0;
            if (condition)
                return (1);
            else
                return (0);
        }

        protected void checkDoor(int doorY, int doorX, int indic)
        {
            if (_player._nbKeys > 0)
            {
                int i = 0;

                if (indic == 5)
                {
                    while (i != _level._nbDoorDraw)
                    {
                        if (_level._doorTab[i]._posX == doorX && _level._doorTab[i]._posY == doorY)
                        {
                            _level._doorTab[i]._posX = -1;
                            _level._doorTab[i]._posY = -1;
                        }
                        i++;
                    }
                    _level._colideMap[doorY][doorX] = 0;
                    _level._colideMap[doorY][doorX + 1] = 0;
                    _level._colideMap[doorY + 1][doorX] = 0;
                    _level._colideMap[doorY + 1][doorX + 1] = 0;
                }
                else if (indic == 6)
                {
                    while (i != _level._nbDoorDraw)
                    {
                        if (_level._doorTab[i]._posX == doorX - 1 && _level._doorTab[i]._posY == doorY)
                        {
                            _level._doorTab[i]._posX = -1;
                            _level._doorTab[i]._posY = -1;
                        }
                        i++;
                    }
                    _level._colideMap[doorY][doorX] = 0;
                    _level._colideMap[doorY][doorX - 1] = 0;
                    _level._colideMap[doorY + 1][doorX] = 0;
                    _level._colideMap[doorY + 1][doorX - 1] = 0;
                }
                else if (indic == 7)
                {
                    while (i != _level._nbDoorDraw)
                    {
                        if (_level._doorTab[i]._posX == doorX && _level._doorTab[i]._posY == doorY - 1)
                        {
                            _level._doorTab[i]._posX = -1;
                            _level._doorTab[i]._posY = -1;
                        }
                        i++;
                    }
                    _level._colideMap[doorY][doorX] = 0;
                    _level._colideMap[doorY][doorX + 1] = 0;
                    _level._colideMap[doorY - 1][doorX] = 0;
                    _level._colideMap[doorY - 1][doorX + 1] = 0;
                }
                else if (indic == 8)
                {
                    while (i != _level._nbDoorDraw)
                    {
                        if (_level._doorTab[i]._posX == doorX - 1 && _level._doorTab[i]._posY == doorY - 1)
                        {
                            _level._doorTab[i]._posX = -1;
                            _level._doorTab[i]._posY = -1;
                        }
                        i++;
                    }
                    _level._colideMap[doorY][doorX] = 0;
                    _level._colideMap[doorY][doorX - 1] = 0;
                    _level._colideMap[doorY - 1][doorX] = 0;
                    _level._colideMap[doorY - 1][doorX - 1] = 0;
                }
                _player._nbKeys -= 1;
                _level._nbDoor -= 1;
                doorOpening.Play();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            if (_inGame && _endlevel == false && !_gameOver) /* In a game */
            {
                #region IG

                #region DRAW_BG
                _spriteBatch.Draw(_level._bg, Vector2.Zero, Color.White);
                #endregion

                #region DRAW_ZOMBIES
                _zombieManager.draw(gameTime, _spriteBatch);
                #endregion

                #region DRAW_KEY
                /* On affiche les clés */
                for (int i = 0; i < _level._nbKeyDraw; i++)
                {
                    _spriteBatch.Draw(_key, new Rectangle(_level._keyTab[i]._posX * 16, _level._keyTab[i]._posY * 16, 16, 16), Color.White);
                }
                #endregion

                #region DRAW_DOOR
                /* On affiche les portes */
                for (int i = 0; i < _level._nbDoorDraw; i++)
                {
                    _spriteBatch.Draw(_door, new Vector2(_level._doorTab[i]._posX * 16, _level._doorTab[i]._posY * 16), Color.White);
                }
                #endregion

                #region LOOP_NOIR
                /* Loop pour afficher le noir */
                bool drawFog = true;
                for (int y = 0; y < _level.Y_MAX; y++)
                {
                    for (int x = 0; x < _level.X_MAX; x++)
                    {
                        drawFog = true;

                        if (_player._direction == 0)
                        {
                            if (x > _player._posX - 5 && x < _player._posX + 5 &&
                                y >= _player._posY && y < _player._posY + 10)
                                drawFog = false;
                        }
                        else if (_player._direction == 1)
                        {
                            if (x >= _player._posX - 10 && x <= _player._posX &&
                                y > _player._posY - 5 && y < _player._posY + 4)
                                drawFog = false;
                        }
                        else if (_player._direction == 2)
                        {
                            if (x > _player._posX - 5 && x < _player._posX + 5 &&
                                y >= _player._posY - 10 && y < _player._posY)
                                drawFog = false;
                        }
                        else if (_player._direction == 3)
                        {
                            if (x >= _player._posX && x <= _player._posX + 10 &&
                                y > _player._posY - 5 && y < _player._posY + 5)
                                drawFog = false;
                        }

                        if (drawFog)
                        {
                            _spriteBatch.Draw(
                            _fog, new Rectangle(
                                (x * 16), (y * 16),
                                _fog.Width, _fog.Height),
                                Color.White);
                        }
                    }
                }
                #endregion

                #region DRAW_LIGHT
                /* On affiche la lumiere */
                if (_player._direction == 0)
                {
                    _spriteBatch.Draw(_torch[0], new Rectangle(
                                (_player._posXpix - 92), (_player._posYpix - 26),
                                _torch[0].Width, _torch[0].Height),
                                Color.White);
                }
                else if (_player._direction == 1)
                {
                    _spriteBatch.Draw(_torch[1], new Rectangle(
                                (_player._posXpix - 198 + 18), (_player._posYpix - 102),
                                _torch[1].Width, _torch[1].Height),
                                Color.White);
                }
                else if (_player._direction == 2)
                {
                    _spriteBatch.Draw(_torch[2], new Rectangle(
                                (_player._posXpix - 89), (_player._posYpix - 187),
                                _torch[2].Width, _torch[2].Height),
                                Color.White);
                }
                else if (_player._direction == 3)
                {
                    _spriteBatch.Draw(_torch[3], new Rectangle(
                                (_player._posXpix - 16), (_player._posYpix - 96),
                                _torch[3].Width, _torch[3].Height),
                                Color.White);
                }
                #endregion

                #region DRAW_PLAYER
                /* On affiche le joueur */
                if (_player.isWalking())
                    this._playerSprite[_player._direction].Draw(gameTime, true, _player._posXpix - 11, _player._posYpix - 25);
                else
                    _spriteBatch.Draw(_player.getTexture(), new Rectangle(_player._posXpix - 11, _player._posYpix - 25, 22, 25), _playerSrcRecSprite[_player._direction], Color.White);
                #endregion

                if (_optionMenu)
                {
                    #region OPTION_PAUSE
                    _menuButton[(int)eMenuButton.MUSIC]._position = new Vector2(300, 170);
                    _menuButton[(int)eMenuButton.SOUND]._position = new Vector2(300, 230);
                    _menuButton[(int)eMenuButton.RETURN]._position = new Vector2(300, 410);
                    _spriteBatch.Draw(_menuButton[(int)eMenuButton.MUSIC].Draw(), _menuButton[(int)eMenuButton.MUSIC]._position, Color.White);
                    _spriteBatch.Draw(_menuButton[(int)eMenuButton.SOUND].Draw(), _menuButton[(int)eMenuButton.SOUND]._position, Color.White);
                    _spriteBatch.Draw(_menuButton[(int)eMenuButton.RETURN].Draw(), _menuButton[(int)eMenuButton.RETURN]._position, Color.White);
                    #endregion
                }
                else if (_inPause) /* Si le jeu est en pause */
                {
                    #region PAUSE
                    _menuButton[(int)eMenuButton.OPTION]._position = new Vector2(300, 290);
                    _menuButton[(int)eMenuButton.RETURN]._position = new Vector2(183, 410);
                    _menuButton[(int)eMenuButton.EXIT]._position = new Vector2(420, 410);
                    _spriteBatch.Draw(_menuButton[(int)eMenuButton.RETURN].Draw(), _menuButton[(int)eMenuButton.RETURN]._position, Color.White);
                    _spriteBatch.Draw(_menuButton[(int)eMenuButton.SAVE].Draw(), _menuButton[(int)eMenuButton.SAVE]._position, Color.White);
                    _spriteBatch.Draw(_menuButton[(int)eMenuButton.OPTION].Draw(), _menuButton[(int)eMenuButton.OPTION]._position, Color.White);
                    _spriteBatch.Draw(_menuButton[(int)eMenuButton.EXIT].Draw(), _menuButton[(int)eMenuButton.EXIT]._position, Color.White);
                    #endregion
                }

                #endregion
            }
            else if (_gameOver)
            {
                #region GAME_OVER
                _menuButton[(int)eMenuButton.NEW_GAME]._position = new Vector2(300, 75);
                _menuButton[(int)eMenuButton.EXIT]._position = new Vector2(300, 495);
                _spriteBatch.Draw(_gameOverBg, Vector2.Zero, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.NEW_GAME].Draw(), _menuButton[(int)eMenuButton.NEW_GAME]._position, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.EXIT].Draw(), _menuButton[(int)eMenuButton.EXIT]._position, Color.White);
                #endregion
            }
            else if (_win)
            {
                #region GAME_OVER
                _menuButton[(int)eMenuButton.NEW_GAME]._position = new Vector2(300, 75);
                _menuButton[(int)eMenuButton.EXIT]._position = new Vector2(300, 495);
                _spriteBatch.Draw(_victoryBg, Vector2.Zero, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.NEW_GAME].Draw(), _menuButton[(int)eMenuButton.NEW_GAME]._position, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.EXIT].Draw(), _menuButton[(int)eMenuButton.EXIT]._position, Color.White);
                #endregion
            }
            else if (_optionMenu)
            {
                #region OPTIONS
                _menuButton[(int)eMenuButton.MUSIC]._position = new Vector2(183, 170);
                _menuButton[(int)eMenuButton.SOUND]._position = new Vector2(183, 230);
                _menuButton[(int)eMenuButton.RETURN]._position = new Vector2(183, 410);
                _spriteBatch.Draw(_bg, Vector2.Zero, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.MUSIC].Draw(), _menuButton[(int)eMenuButton.MUSIC]._position, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.SOUND].Draw(), _menuButton[(int)eMenuButton.SOUND]._position, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.RETURN].Draw(), _menuButton[(int)eMenuButton.RETURN]._position, Color.White);

                #endregion
            }
            else if (_creditPage)
            {
                #region OPTIONS
                _menuButton[(int)eMenuButton.RETURN]._position = new Vector2(483, 410);
                _spriteBatch.Draw(_creditsBg, Vector2.Zero, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.RETURN].Draw(), _menuButton[(int)eMenuButton.RETURN]._position, Color.White);

                #endregion
            }
            else
            {
                #region NOT_IG
                _menuButton[(int)eMenuButton.NEW_GAME]._position = new Vector2(183, 170);
                _menuButton[(int)eMenuButton.OPTION]._position = new Vector2(183, 290);
                _menuButton[(int)eMenuButton.EXIT]._position = new Vector2(183, 410);
                _spriteBatch.Draw(_bg, Vector2.Zero, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.NEW_GAME].Draw(), _menuButton[(int)eMenuButton.NEW_GAME]._position, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.LOAD].Draw(), _menuButton[(int)eMenuButton.LOAD]._position, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.OPTION].Draw(), _menuButton[(int)eMenuButton.OPTION]._position, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.CREDIT].Draw(), _menuButton[(int)eMenuButton.CREDIT]._position, Color.White);
                _spriteBatch.Draw(_menuButton[(int)eMenuButton.EXIT].Draw(), _menuButton[(int)eMenuButton.EXIT]._position, Color.White);
                #endregion
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
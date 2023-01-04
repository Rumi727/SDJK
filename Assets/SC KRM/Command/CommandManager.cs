using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Tree;
using Cysharp.Threading.Tasks;
using SCKRM.Input;
using SCKRM.Renderer;
using SCKRM.Sound;
using SCKRM.UI;
using SCKRM.UI.Overlay;
using SCKRM.UI.StatusBar;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCKRM.Command
{
    public sealed class CommandManager : Manager<CommandManager>
    {
        public static CommandDispatcher<DefaultCommandSource> commandDispatcher { get; } = new CommandDispatcher<DefaultCommandSource>();
        public static DefaultCommandSource defaultCommandSource { get; set; } = new DefaultCommandSource();

        public static bool isChatShow { get; private set; } = false;

        [SerializeField] TMP_InputField _inputField; public TMP_InputField inputField => _inputField;
        [SerializeField] GameObject _chat; public GameObject chat => _chat;
        [SerializeField] CanvasGroup _commandCanvasGroup; public CanvasGroup commandCanvasGroup => _commandCanvasGroup;

        void Awake()
        {
            if (SingletonCheck(this))
            {
                CommandSyntaxException.BuiltInExceptions = new BuiltInExceptions();
                CommandSyntaxException.ContextAmount = 32;

                RegisterExecute();

                RegisterLog();

                RegisterGameSpeed();

                RegisterPlaySound();
                RegisterStopSound();
                RegisterStopSoundAll();

                RegisterPlayNBS();
                RegisterStopNBS();
                RegisterStopNBSAll();

                RegisterAllRefresh();
                RegisterAllRerender();
                RegisterAllTextRerender();
            }
        }

        void Update()
        {
            if (!InitialLoadManager.isInitialLoadEnd)
                return;

            if (isChatShow)
            {
                commandCanvasGroup.alpha = commandCanvasGroup.alpha.Lerp(1, 0.2f * Kernel.fpsUnscaledDeltaTime);
                if (commandCanvasGroup.alpha > 0.99f)
                    commandCanvasGroup.alpha = 1;

                commandCanvasGroup.interactable = true;
                commandCanvasGroup.blocksRaycasts = true;

                if (!chat.activeSelf)
                    chat.SetActive(true);

                if (!instance.inputField.isFocused)
                    instance.inputField.Select();

                if (InputManager.GetKey("gui.ok", InputType.Down, InputManager.inputLockDenyAllForceInput))
                {
                    if (!string.IsNullOrWhiteSpace(inputField.text))
                        Execute(inputField.text);

                    inputField.text = "";
                    Hide().Forget();
                }
            }
            else
            {
                if (InputManager.GetKey("command_manager.chat_show", InputType.Down, InputManager.inputLockDenyAll))
                    Show().Forget();

                commandCanvasGroup.alpha = commandCanvasGroup.alpha.Lerp(0, 0.2f * Kernel.fpsUnscaledDeltaTime);
                if (commandCanvasGroup.alpha < 0.01f)
                {
                    commandCanvasGroup.alpha = 0;

                    if (chat.activeSelf)
                        chat.SetActive(false);
                }

                commandCanvasGroup.interactable = false;
                commandCanvasGroup.blocksRaycasts = false;
            }
        }


        static GameObject previouslyTabSelectGameObject = null;
        static GameObject previouslySelectedGameObject = null;
        static bool previouslyForceInputLock = false;
        public static async UniTask Show()
        {
            if (isChatShow)
                return;

            await UniTask.WaitUntil(() => instance != null);

            isChatShow = true;

            previouslyForceInputLock = StatusBarManager.tabSelectGameObject;
            previouslyForceInputLock = InputManager.forceInputLock;
            previouslySelectedGameObject = EventSystem.current.currentSelectedGameObject;

            instance.inputField.text = "";

            InputManager.forceInputLock = true;
            UIManager.BackEventAdd(hide, true);
        }

        public static async UniTask Hide()
        {
            if (!isChatShow)
                return;

            await UniTask.WaitUntil(() => instance != null);

            isChatShow = false;

            StatusBarManager.tabSelectGameObject = previouslyTabSelectGameObject;
            InputManager.forceInputLock = previouslyForceInputLock;
            EventSystem.current.SetSelectedGameObject(previouslySelectedGameObject);

            UIManager.BackEventRemove(hide, true);
        }

        static void hide() => Hide().Forget();

        public static void Execute(string input)
        {
            try
            {
                defaultCommandSource.Initialization();
                commandDispatcher.Execute(input, defaultCommandSource);
                defaultCommandSource.ExecuteAtTheEndInvoke();
            }
            catch (CommandSyntaxException e)
            {
                Debug.LogException(e.GetCustomException());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        static void RegisterExecute()
        {
            CommandNode<DefaultCommandSource> root = commandDispatcher.GetRoot();
            CommandNode<DefaultCommandSource> execute = 
            commandDispatcher.Register(x =>
                x.Literal("execute")
                    .Then(x =>
                        x.Literal("run")
                            .Redirect(root)
                    )
            );

            commandDispatcher.Register(x =>
                    x.Literal("execute")
                        .Then(x =>
                            x.Literal("positioned")
                                .Then(x =>
                                    x.Argument("pos", Arguments.Vector3())
                                        .Redirect(execute, x =>
                                        {
                                            Vector3 pos = Arguments.GetVector3(x, "pos", x.Source.currentPosition);
                                            float magnitude = pos.magnitude;

                                            x.Source.currentPosition = pos;
                                            x.Source.lastCommandResult = new CommandResult(true, pos, magnitude);

                                            return x.Source;
                                        })
                                )
                        )
                        .Then(x =>
                            x.Literal("align")
                                .Then(x =>
                                    x.Argument("axes", Arguments.PosSwizzle())
                                        .Redirect(execute, x =>
                                        {
                                            Arguments.PosSwizzleEnum posSwizzle = Arguments.GetPosSwizzle(x, "axes");

                                            Vector3 position = x.Source.currentPosition;
                                            if (posSwizzle.HasFlag(Arguments.PosSwizzleEnum.x))
                                                position.x = position.x.Floor();
                                            if (posSwizzle.HasFlag(Arguments.PosSwizzleEnum.y))
                                                position.y = position.y.Floor();
                                            if (posSwizzle.HasFlag(Arguments.PosSwizzleEnum.z))
                                                position.z = position.z.Floor();

                                            x.Source.currentPosition = position;
                                            x.Source.lastCommandResult = new CommandResult(true, posSwizzle, (int)posSwizzle);

                                            return x.Source;
                                        })
                                )
                        )
                        .Then(x =>
                            x.Literal("facing")
                                .Then(x =>
                                    x.Argument("pos", Arguments.Vector3())
                                        .Redirect(execute, x =>
                                        {
                                            Vector3 pos = Arguments.GetVector3(x, "pos", x.Source.currentPosition);
                                            Vector3 rotation = Quaternion.LookRotation(pos).eulerAngles;
                                            float magnitude = rotation.magnitude;

                                            x.Source.currentPosition = pos;
                                            x.Source.lastCommandResult = new CommandResult(true, rotation, rotation.magnitude);

                                            return x.Source;
                                        })
                                )
                        )
                        .Then(x =>
                            x.Literal("rotated")
                                .Then(x =>
                                    x.Argument("rot", Arguments.Vector3())
                                        .Redirect(execute, x =>
                                        {
                                            Vector3 rotation = Arguments.GetVector3(x, "rot", x.Source.currentRotation);
                                            float magnitude = rotation.magnitude;

                                            x.Source.currentRotation = rotation;
                                            x.Source.lastCommandResult = new CommandResult(true, rotation, magnitude);

                                            return x.Source;
                                        })
                                )
                        )
                );
        }

        static void RegisterLog()
        {
            commandDispatcher.Register(x =>
                x.Literal("log")
                    .Then(x =>
                        x.Argument("text", Arguments.String())
                            .Executes(x =>
                            {
                                string text = Arguments.GetString(x, "text");
                                Debug.ForceLog(text, nameof(CommandManager));

                                x.Source.lastCommandResult = new CommandResult(true, text, text.Length, text);
                                return text.Length;
                            })
                    )
            );
        }

        static void RegisterGameSpeed()
        {
            commandDispatcher.Register(x =>
                x.Literal("gamespeed")
                    .Then(x =>
                        x.Argument("value", Arguments.Float())
                            .Executes(x =>
                            {
                                Kernel.gameSpeed = Arguments.GetFloat(x, "value");

                                string log = CommandLanguage.SearchLanguage("change_to").Replace("%name%", nameof(Kernel.gameSpeed)).Replace("%value%", Kernel.gameSpeed.ToString());
                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                return 1;
                            })
                    )
                    .Executes(x =>
                    {
                        string log = CommandLanguage.SearchLanguage("value_of").Replace("%name%", nameof(Kernel.gameSpeed)).Replace("%value%", Kernel.gameSpeed.ToString());
                        x.Source.lastCommandResult = new CommandResult(true, Kernel.gameSpeed, Kernel.gameSpeed, log);
                        return (int)Kernel.gameSpeed;
                    })
            );
        }

        static void RegisterPlaySound()
        {
            commandDispatcher.Register(x =>
                x.Literal("playsound")
                    .Then(x =>
                        x.Argument("sound", Arguments.NameSpacePathPair())
                            .Then(x =>
                                x.Argument("volume", Arguments.Float())
                                    .Then(x =>
                                        x.Argument("loop", Arguments.Bool())
                                            .Then(x =>
                                                x.Argument("pitch", Arguments.Float())
                                                    .Then(x =>
                                                        x.Argument("tempo", Arguments.Float())
                                                            .Then(x =>
                                                                x.Argument("panStereo", Arguments.Float(-1, 1))
                                                                    .Then(x =>
                                                                        x.Argument("minDistance", Arguments.Float(0))
                                                                            .Then(x =>
                                                                                x.Argument("maxDistance", Arguments.Float(0))
                                                                                    .Then(x =>
                                                                                        x.Argument("position", Arguments.Vector3())
                                                                                            .Executes(x =>
                                                                                            {
                                                                                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                                                                                float volume = Arguments.GetFloat(x, "volume");
                                                                                                bool loop = Arguments.GetBool(x, "loop");
                                                                                                float pitch = Arguments.GetFloat(x, "pitch");
                                                                                                float tempo = Arguments.GetFloat(x, "tempo");
                                                                                                float panStereo = Arguments.GetFloat(x, "panStereo");
                                                                                                float minDistance = Arguments.GetFloat(x, "minDistance");
                                                                                                float maxDistance = Arguments.GetFloat(x, "maxDistance");
                                                                                                Vector3 position = Arguments.GetVector3(x, "position", x.Source.currentPosition);

                                                                                                SoundManager.PlaySound(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo, panStereo, minDistance, maxDistance, null, position.x, position.y, position.z);

                                                                                                string log = CommandLanguage.SearchLanguage("play_sound");
                                                                                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                                                return 1;
                                                                                            })
                                                                                    )
                                                                                    .Executes(x =>
                                                                                    {
                                                                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                                                                        float volume = Arguments.GetFloat(x, "volume");
                                                                                        bool loop = Arguments.GetBool(x, "loop");
                                                                                        float pitch = Arguments.GetFloat(x, "pitch");
                                                                                        float tempo = Arguments.GetFloat(x, "tempo");
                                                                                        float panStereo = Arguments.GetFloat(x, "panStereo");
                                                                                        float minDistance = Arguments.GetFloat(x, "minDistance");
                                                                                        float maxDistance = Arguments.GetFloat(x, "maxDistance");

                                                                                        SoundManager.PlaySound(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo, panStereo, minDistance, maxDistance);

                                                                                        string log = CommandLanguage.SearchLanguage("play_sound");
                                                                                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                                        return 1;
                                                                                    })
                                                                            )
                                                                            .Executes(x =>
                                                                            {
                                                                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                                                                float volume = Arguments.GetFloat(x, "volume");
                                                                                bool loop = Arguments.GetBool(x, "loop");
                                                                                float pitch = Arguments.GetFloat(x, "pitch");
                                                                                float tempo = Arguments.GetFloat(x, "tempo");
                                                                                float panStereo = Arguments.GetFloat(x, "panStereo");
                                                                                float minDistance = Arguments.GetFloat(x, "minDistance");

                                                                                SoundManager.PlaySound(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo, panStereo, minDistance);

                                                                                string log = CommandLanguage.SearchLanguage("play_sound");
                                                                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                                return 1;
                                                                            })
                                                                    )
                                                                    .Executes(x =>
                                                                    {
                                                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                                                        float volume = Arguments.GetFloat(x, "volume");
                                                                        bool loop = Arguments.GetBool(x, "loop");
                                                                        float pitch = Arguments.GetFloat(x, "pitch");
                                                                        float tempo = Arguments.GetFloat(x, "tempo");
                                                                        float panStereo = Arguments.GetFloat(x, "panStereo");

                                                                        SoundManager.PlaySound(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo, panStereo);

                                                                        string log = CommandLanguage.SearchLanguage("play_sound");
                                                                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                        return 1;
                                                                    })
                                                            )
                                                            .Executes(x =>
                                                            {
                                                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                                                float volume = Arguments.GetFloat(x, "volume");
                                                                bool loop = Arguments.GetBool(x, "loop");
                                                                float pitch = Arguments.GetFloat(x, "pitch");
                                                                float tempo = Arguments.GetFloat(x, "tempo");

                                                                SoundManager.PlaySound(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo);

                                                                string log = CommandLanguage.SearchLanguage("play_sound");
                                                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                return 1;
                                                            })
                                                    )
                                                    .Executes(x =>
                                                    {
                                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                                        float volume = Arguments.GetFloat(x, "volume");
                                                        bool loop = Arguments.GetBool(x, "loop");
                                                        float pitch = Arguments.GetFloat(x, "pitch");

                                                        SoundManager.PlaySound(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch);

                                                        string log = CommandLanguage.SearchLanguage("play_sound");
                                                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                        return 1;
                                                    })
                                            )
                                            .Executes(x =>
                                            {
                                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                                float volume = Arguments.GetFloat(x, "volume");
                                                bool loop = Arguments.GetBool(x, "loop");

                                                SoundManager.PlaySound(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop);

                                                string log = CommandLanguage.SearchLanguage("play_sound");
                                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                return 1;
                                            })
                                    )
                                    .Executes(x =>
                                    {
                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                        float volume = Arguments.GetFloat(x, "volume");

                                        SoundManager.PlaySound(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume);

                                        string log = CommandLanguage.SearchLanguage("play_sound");
                                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                        return 1;
                                    })
                            )
                            .Executes(x =>
                            {
                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                SoundManager.PlaySound(nameSpacePathPair.path, nameSpacePathPair.nameSpace);

                                string log = CommandLanguage.SearchLanguage("play_sound");
                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                return 1;
                            })
                    )
            );
        }

        static void RegisterStopSound()
        {
            commandDispatcher.Register(x =>
                x.Literal("stopsound")
                    .Then(x =>
                        x.Argument("sound", Arguments.NameSpacePathPair())
                            .Then(x =>
                                x.Argument("all", Arguments.Bool())
                                    .Executes(x =>
                                    {
                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                        bool all = Arguments.GetBool(x, "all");
                                        int stopCount = SoundManager.StopSound(nameSpacePathPair.path, nameSpacePathPair.nameSpace, all);

                                        string log = CommandLanguage.SearchLanguage("stop_sound");
                                        x.Source.lastCommandResult = new CommandResult(true, stopCount, stopCount, log);
                                        return stopCount;
                                    })
                            )
                            .Executes(x =>
                            {
                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "sound");
                                int stopCount = SoundManager.StopSound(nameSpacePathPair.path, nameSpacePathPair.nameSpace);

                                string log = CommandLanguage.SearchLanguage("stop_sound");
                                x.Source.lastCommandResult = new CommandResult(true, stopCount, stopCount, log);
                                return stopCount;
                            })
                    )
            );
        }

        static void RegisterStopSoundAll()
        {
            commandDispatcher.Register(x =>
                x.Literal("stopsoundall")
                    .Then(x =>
                        x.Argument("bgm", Arguments.Bool())
                            .Executes(x =>
                            {
                                int stopCount = SoundManager.StopSoundAll(Arguments.GetBool(x, "bgm"));

                                string log = CommandLanguage.SearchLanguage("stop_sound_all");
                                x.Source.lastCommandResult = new CommandResult(true, stopCount, stopCount, log);
                                return stopCount;
                            })
                    )
                    .Executes(x =>
                    {
                        int stopCount = SoundManager.StopSoundAll();

                        string log = CommandLanguage.SearchLanguage("stop_sound_all");
                        x.Source.lastCommandResult = new CommandResult(true, stopCount, stopCount, log);
                        return stopCount;
                    })
            );
        }

        static void RegisterPlayNBS()
        {
            commandDispatcher.Register(x =>
                x.Literal("playnbs")
                    .Then(x =>
                        x.Argument("nbs", Arguments.NameSpacePathPair())
                            .Then(x =>
                                x.Argument("volume", Arguments.Float())
                                    .Then(x =>
                                        x.Argument("loop", Arguments.Bool())
                                            .Then(x =>
                                                x.Argument("pitch", Arguments.Float())
                                                    .Then(x =>
                                                        x.Argument("tempo", Arguments.Float())
                                                            .Then(x =>
                                                                x.Argument("panStereo", Arguments.Float(-1, 1))
                                                                    .Then(x =>
                                                                        x.Argument("minDistance", Arguments.Float(0))
                                                                            .Then(x =>
                                                                                x.Argument("maxDistance", Arguments.Float(0))
                                                                                    .Then(x =>
                                                                                        x.Argument("position", Arguments.Vector3())
                                                                                            .Executes(x =>
                                                                                            {
                                                                                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                                                                                float volume = Arguments.GetFloat(x, "volume");
                                                                                                bool loop = Arguments.GetBool(x, "loop");
                                                                                                float pitch = Arguments.GetFloat(x, "pitch");
                                                                                                float tempo = Arguments.GetFloat(x, "tempo");
                                                                                                float panStereo = Arguments.GetFloat(x, "panStereo");
                                                                                                float minDistance = Arguments.GetFloat(x, "minDistance");
                                                                                                float maxDistance = Arguments.GetFloat(x, "maxDistance");
                                                                                                Vector3 position = Arguments.GetVector3(x, "position");

                                                                                                SoundManager.PlayNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo, panStereo, minDistance, maxDistance, null, position.x, position.y, position.z);

                                                                                                string log = CommandLanguage.SearchLanguage("play_nbs");
                                                                                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                                                return 1;
                                                                                            })
                                                                                    )
                                                                                    .Executes(x =>
                                                                                    {
                                                                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                                                                        float volume = Arguments.GetFloat(x, "volume");
                                                                                        bool loop = Arguments.GetBool(x, "loop");
                                                                                        float pitch = Arguments.GetFloat(x, "pitch");
                                                                                        float tempo = Arguments.GetFloat(x, "tempo");
                                                                                        float panStereo = Arguments.GetFloat(x, "panStereo");
                                                                                        float minDistance = Arguments.GetFloat(x, "minDistance");
                                                                                        float maxDistance = Arguments.GetFloat(x, "maxDistance");

                                                                                        SoundManager.PlayNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo, panStereo, minDistance, maxDistance);

                                                                                        string log = CommandLanguage.SearchLanguage("play_nbs");
                                                                                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                                        return 1;
                                                                                    })
                                                                            )
                                                                            .Executes(x =>
                                                                            {
                                                                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                                                                float volume = Arguments.GetFloat(x, "volume");
                                                                                bool loop = Arguments.GetBool(x, "loop");
                                                                                float pitch = Arguments.GetFloat(x, "pitch");
                                                                                float tempo = Arguments.GetFloat(x, "tempo");
                                                                                float panStereo = Arguments.GetFloat(x, "panStereo");
                                                                                float minDistance = Arguments.GetFloat(x, "minDistance");

                                                                                SoundManager.PlayNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo, panStereo, minDistance);

                                                                                string log = CommandLanguage.SearchLanguage("play_nbs");
                                                                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                                return 1;
                                                                            })
                                                                    )
                                                                    .Executes(x =>
                                                                    {
                                                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                                                        float volume = Arguments.GetFloat(x, "volume");
                                                                        bool loop = Arguments.GetBool(x, "loop");
                                                                        float pitch = Arguments.GetFloat(x, "pitch");
                                                                        float tempo = Arguments.GetFloat(x, "tempo");
                                                                        float panStereo = Arguments.GetFloat(x, "panStereo");

                                                                        SoundManager.PlayNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo, panStereo);

                                                                        string log = CommandLanguage.SearchLanguage("play_nbs");
                                                                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                        return 1;
                                                                    })
                                                            )
                                                            .Executes(x =>
                                                            {
                                                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                                                float volume = Arguments.GetFloat(x, "volume");
                                                                bool loop = Arguments.GetBool(x, "loop");
                                                                float pitch = Arguments.GetFloat(x, "pitch");
                                                                float tempo = Arguments.GetFloat(x, "tempo");

                                                                SoundManager.PlayNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch, tempo);

                                                                string log = CommandLanguage.SearchLanguage("play_nbs");
                                                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                                return 1;
                                                            })
                                                    )
                                                    .Executes(x =>
                                                    {
                                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                                        float volume = Arguments.GetFloat(x, "volume");
                                                        bool loop = Arguments.GetBool(x, "loop");
                                                        float pitch = Arguments.GetFloat(x, "pitch");

                                                        SoundManager.PlayNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop, pitch);

                                                        string log = CommandLanguage.SearchLanguage("play_nbs");
                                                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                        return 1;
                                                    })
                                            )
                                            .Executes(x =>
                                            {
                                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                                float volume = Arguments.GetFloat(x, "volume");
                                                bool loop = Arguments.GetBool(x, "loop");

                                                SoundManager.PlayNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume, loop);

                                                string log = CommandLanguage.SearchLanguage("play_nbs");
                                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                                return 1;
                                            })
                                    )
                                    .Executes(x =>
                                    {
                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                        float volume = Arguments.GetFloat(x, "volume");

                                        SoundManager.PlayNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace, volume);

                                        string log = CommandLanguage.SearchLanguage("play_nbs");
                                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                        return 1;
                                    })
                            )
                            .Executes(x =>
                            {
                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                SoundManager.PlayNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace);

                                string log = CommandLanguage.SearchLanguage("play_nbs");
                                x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                                return 1;
                            })
                    )
            );
        }

        static void RegisterStopNBS()
        {
            commandDispatcher.Register(x =>
                x.Literal("stopnbs")
                    .Then(x =>
                        x.Argument("nbs", Arguments.NameSpacePathPair())
                            .Then(x =>
                                x.Argument("all", Arguments.Bool())
                                    .Executes(x =>
                                    {
                                        NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                        bool all = Arguments.GetBool(x, "all");
                                        int stopCount = SoundManager.StopNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace, all);

                                        string log = CommandLanguage.SearchLanguage("stop_nbs");
                                        x.Source.lastCommandResult = new CommandResult(true, stopCount, stopCount, log);
                                        return stopCount;
                                    })
                            )
                            .Executes(x =>
                            {
                                NameSpacePathPair nameSpacePathPair = Arguments.GetNameSpacePathPair(x, "nbs");
                                int stopCount = SoundManager.StopNBS(nameSpacePathPair.path, nameSpacePathPair.nameSpace);

                                string log = CommandLanguage.SearchLanguage("stop_nbs");
                                x.Source.lastCommandResult = new CommandResult(true, stopCount, stopCount, log);
                                return stopCount;
                            })
                    )
            );
        }

        static void RegisterStopNBSAll()
        {
            commandDispatcher.Register(x =>
                x.Literal("stopnbsall")
                    .Then(x =>
                        x.Argument("bgm", Arguments.Bool())
                            .Executes(x =>
                            {
                                int stopCount = SoundManager.StopNBSAll(Arguments.GetBool(x, "bgm"));

                                string log = CommandLanguage.SearchLanguage("stop_nbs_all");
                                x.Source.lastCommandResult = new CommandResult(true, stopCount, stopCount, log);
                                return stopCount;
                            })
                    )
                    .Executes(x =>
                    {
                        int stopCount = SoundManager.StopNBSAll();

                        string log = CommandLanguage.SearchLanguage("stop_nbs_all");
                        x.Source.lastCommandResult = new CommandResult(true, stopCount, stopCount, log);
                        return SoundManager.StopNBSAll();
                    })
            );
        }

        static void RegisterAllRefresh()
        {
            commandDispatcher.Register(x =>
                x.Literal("allrefresh")
                    .Executes(x =>
                    {
                        Kernel.AllRefresh().Forget();

                        string log = CommandLanguage.SearchLanguage("all_refresh");
                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                        return 1;
                    })
            );
        }

        static void RegisterAllRerender()
        {
            commandDispatcher.Register(x =>
                x.Literal("allrenderer")
                    .Executes(x =>
                    {
                        RendererManager.AllRerender();

                        string log = CommandLanguage.SearchLanguage("all_rerender");
                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                        return 1;
                    })
            );
        }

        static void RegisterAllTextRerender()
        {
            commandDispatcher.Register(x =>
                x.Literal("alltextrenderer")
                    .Executes(x =>
                    {
                        RendererManager.AllTextRerender();

                        string log = CommandLanguage.SearchLanguage("all_text_rerender");
                        x.Source.lastCommandResult = new CommandResult(true, 1, 1, log);
                        return 1;
                    })
            );
        }
    }

    public class DefaultCommandSource
    {
        public CommandResult lastCommandResult { get; set; } = new CommandResult();

        public virtual Vector3 currentPosition { get; set; } = Vector3.zero;
        public virtual Vector3 currentRotation { get; set; } = Vector3.zero;



        public event ExecuteAtTheEnd executeAtTheEnd;
        public delegate void ExecuteAtTheEnd(DefaultCommandSource source);

        public void ExecuteAtTheEndInvoke() => executeAtTheEnd?.Invoke(this);
        public virtual void Initialization()
        {
            lastCommandResult = new CommandResult();

            currentPosition = Vector3.zero;
            currentRotation = Vector3.zero;
        }
    }

    public struct CommandResult
    {
        public CommandResult(bool isSuccess = false, object result = null, double resultNumber = 0, string logText = "")
        {
            this.isSuccess = isSuccess;

            this.result = result;
            this.resultNumber = resultNumber;

            this.logText = logText;
        }

        public bool isSuccess { get; }

        public object result { get; }
        public double resultNumber { get; }

        public string logText { get; }
    }
}

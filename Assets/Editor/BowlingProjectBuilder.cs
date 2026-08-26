using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BowlingProjectBuilder
{
    const string Scenes="Assets/Scenes/"; static Font font;
    [InitializeOnLoadMethod]
    static void AutoBuild()
    {
        if (!System.IO.File.Exists(System.IO.Path.GetFullPath(Scenes+"MainMenu.unity")))
            EditorApplication.delayCall += Build;
    }
    [MenuItem("Bowling 1302/Rebuild Game Scenes")]
    public static void Build()
    {
        font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); BuildMenu(); BuildGame();
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scenes+"MainMenu.unity",true),new EditorBuildSettingsScene(Scenes+"BowlingGame.unity",true)};
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); Debug.Log("Bowling 1302 scenes rebuilt successfully.");
    }
    public static void BuildWindowsDevelopment()
    {
        Build();
        System.IO.Directory.CreateDirectory("Builds/Windows");
        var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes=new[]{Scenes+"MainMenu.unity",Scenes+"BowlingGame.unity"},
            locationPathName="Builds/Windows/Bowling1302.exe",
            target=BuildTarget.StandaloneWindows64,
            options=BuildOptions.Development
        });
        if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new System.Exception("Windows build failed: "+report.summary.result);
        Debug.Log($"Windows build succeeded: {report.summary.totalSize} bytes");
    }
    static void BuildMenu()
    {
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single); AddCamera(new Color(.025f,.015f,.12f)); AddLight();
        var canvas=MakeCanvas(); var root=new GameObject("MainMenuController"); var c=root.AddComponent<MainMenuController>();
        var home=Panel(canvas.transform,"Home",new Color(.03f,.02f,.14f,.94f));
        Label(home.transform,"Title","BOWLING\n<size=62><color=#42E8FF>1302</color></size>",84,new Vector2(0,190),new Vector2(900,220));
        Label(home.transform,"Tagline","NEON LANES • TEN FRAMES • ONE PERFECT GAME",20,new Vector2(0,78),new Vector2(800,45));
        MakeButton(home.transform,"Play",new Vector2(0,5),c.Play); MakeButton(home.transform,"Settings",new Vector2(0,-65),c.ShowSettings);
        MakeButton(home.transform,"Credits",new Vector2(0,-135),c.ShowCredits); MakeButton(home.transform,"Quit",new Vector2(0,-205),c.Quit);
        var settings=Panel(canvas.transform,"Settings",new Color(.02f,.035f,.15f,.98f)); settings.SetActive(false);
        Label(settings.transform,"Heading","SETTINGS",52,new Vector2(0,250),new Vector2(700,80));
        MakeSlider(settings.transform,"MASTER VOLUME",new Vector2(0,155),.8f,c.SetMaster); MakeSlider(settings.transform,"MUSIC VOLUME",new Vector2(0,100),.65f,c.SetMusic); MakeSlider(settings.transform,"SFX VOLUME",new Vector2(0,45),.85f,c.SetSfx);
        MakeToggle(settings.transform,"FULLSCREEN",new Vector2(0,-20),true,c.SetFullscreen); MakeDropdown(settings.transform,"QUALITY",new Vector2(0,-85),QualitySettings.names,c.SetQuality);
        var rs=Screen.resolutions; var opts=new string[rs.Length]; for(int i=0;i<rs.Length;i++)opts[i]=$"{rs[i].width} × {rs[i].height}";
        MakeDropdown(settings.transform,"RESOLUTION",new Vector2(0,-150),opts,c.SetResolution); MakeButton(settings.transform,"Back",new Vector2(0,-250),c.ShowHome);
        var credits=Panel(canvas.transform,"Credits",new Color(.04f,.015f,.13f,.98f)); credits.SetActive(false);
        Label(credits.transform,"Heading","CREDITS",52,new Vector2(0,230),new Vector2(700,80));
        Label(credits.transform,"Copy","Designed for the Bowling 1302 project\nBuilt with Unity\nAudio-ready arcade framework\n\nThanks to the Unity community and open-source contributors.",24,Vector2.zero,new Vector2(900,300));
        MakeButton(credits.transform,"Back",new Vector2(0,-250),c.ShowHome); Set(c,"home",home);Set(c,"settings",settings);Set(c,"credits",credits);
        EditorSceneManager.SaveScene(scene,Scenes+"MainMenu.unity");
    }
    static void BuildGame()
    {
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single); RenderSettings.ambientLight=new Color(.12f,.14f,.25f);
        var cam=AddCamera(new Color(.015f,.02f,.07f)); cam.transform.SetPositionAndRotation(new Vector3(0,7,-10),Quaternion.Euler(20,0,0)); cam.fieldOfView=55; AddLight();
        Cube("Lane",new Vector3(0,0,9),new Vector3(3.6f,.2f,28),new Color(.84f,.53f,.2f));
        Cube("Left Gutter",new Vector3(-2.05f,-.12f,9),new Vector3(.5f,.25f,28),new Color(.04f,.08f,.18f)); Cube("Right Gutter",new Vector3(2.05f,-.12f,9),new Vector3(.5f,.25f,28),new Color(.04f,.08f,.18f));
        Cube("Backstop",new Vector3(0,1.5f,23),new Vector3(5,3,.3f),new Color(.1f,.02f,.2f));
        for(int i=0;i<8;i++) Cube("Neon Accent",new Vector3(i%2==0?-2.35f:2.35f,.12f,-3+i*3.5f),new Vector3(.08f,.08f,2.2f),i%4==0?Color.cyan:new Color(1,.1f,.65f));
        var ball=GameObject.CreatePrimitive(PrimitiveType.Sphere); ball.name="Bowling Ball"; ball.transform.position=new Vector3(0,.42f,-4); ball.transform.localScale=Vector3.one*.8f; ball.GetComponent<Renderer>().sharedMaterial=Mat(new Color(.1f,.35f,1));
        var rb=ball.AddComponent<Rigidbody>();rb.mass=7;rb.collisionDetectionMode=CollisionDetectionMode.Continuous; var bc=ball.AddComponent<BowlingBallController>();
        var guide=new GameObject("Aim Guide").AddComponent<LineRenderer>();guide.positionCount=2;guide.SetPositions(new[]{new Vector3(0,.03f,-3),new Vector3(0,.03f,5)});guide.startWidth=.04f;guide.endWidth=.015f;guide.material=Mat(Color.cyan);
        var deck=new GameObject("Pin Deck"); deck.AddComponent<PinDeckController>(); int id=1;
        for(int row=0;row<4;row++)for(int col=0;col<=row;col++) { var pin=GameObject.CreatePrimitive(PrimitiveType.Capsule);pin.name=$"Pin {id++}";pin.transform.SetParent(deck.transform);pin.transform.position=new Vector3((col-row*.5f)*.62f,.62f,17+row*.62f);pin.transform.localScale=new Vector3(.42f,.62f,.42f);pin.GetComponent<Renderer>().sharedMaterial=Mat(Color.white);var pr=pin.AddComponent<Rigidbody>();pr.mass=1.5f;pr.centerOfMass=new Vector3(0,-.25f,0); }
        var dead=new GameObject("Dead Zone");dead.transform.position=new Vector3(0,0,22);
        var canvas=MakeCanvas(); var hud=canvas.gameObject.AddComponent<BowlingHUD>();
        var status=Label(canvas.transform,"Status","FRAME 1 • ROLL 1",24,new Vector2(0,330),new Vector2(800,45)); var score=Label(canvas.transform,"Scorecard","",18,new Vector2(0,245),new Vector2(1180,120));
        Label(canvas.transform,"Controls","A / D  MOVE     Q / E  AIM     HOLD + RELEASE SPACE  POWER     ESC  PAUSE",18,new Vector2(0,-330),new Vector2(1100,40)); var feedback=Label(canvas.transform,"Feedback","",44,new Vector2(0,80),new Vector2(700,80));
        var powerBg=UiImage(canvas.transform,"Power",new Color(.02f,.02f,.08f,.85f),new Vector2(-430,-270),new Vector2(260,28)); var fill=UiImage(powerBg.transform,"Fill",new Color(1,.15f,.6f),Vector2.zero,new Vector2(250,18));fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;
        var managerGo=new GameObject("Bowling Game Manager");managerGo.AddComponent<AudioSource>();managerGo.AddComponent<ArcadeAudio>();var gm=managerGo.AddComponent<BowlingGameManager>();
        var pause=Panel(canvas.transform,"Pause Panel",new Color(.015f,.02f,.09f,.96f));Label(pause.transform,"Pause","PAUSED",58,new Vector2(0,185),new Vector2(500,80));
        MakeButton(pause.transform,"Resume",new Vector2(0,70),gm.TogglePause);MakeButton(pause.transform,"Restart Match",Vector2.zero,gm.RestartMatch);MakeButton(pause.transform,"Main Menu",new Vector2(0,-70),gm.MainMenu);Label(pause.transform,"SettingsTip","Display and audio settings are available from the Main Menu.",18,new Vector2(0,-170),new Vector2(800,50));pause.SetActive(false);
        var results=Panel(canvas.transform,"Results Panel",new Color(.03f,.01f,.13f,.97f));var resultText=Label(results.transform,"Result","FINAL SCORE",58,new Vector2(0,80),new Vector2(700,180));var countdown=Label(results.transform,"Countdown","NEW MATCH IN 5 SECONDS",22,new Vector2(0,-80),new Vector2(700,50));MakeButton(results.transform,"Main Menu",new Vector2(0,-190),gm.MainMenu);results.SetActive(false);
        Set(hud,"statusText",status);Set(hud,"scoreText",score);Set(hud,"feedbackText",feedback);Set(hud,"resultsText",resultText);Set(hud,"countdownText",countdown);Set(hud,"powerFill",fill);Set(hud,"pausePanel",pause);Set(hud,"resultsPanel",results);
        Set(gm,"ball",bc);Set(gm,"deck",deck.GetComponent<PinDeckController>());Set(gm,"hud",hud);Set(gm,"deadZone",dead.transform);
        EditorSceneManager.SaveScene(scene,Scenes+"BowlingGame.unity");
    }
    static Camera AddCamera(Color bg){var g=new GameObject("Main Camera");g.tag="MainCamera";var c=g.AddComponent<Camera>();c.clearFlags=CameraClearFlags.SolidColor;c.backgroundColor=bg;g.AddComponent<AudioListener>();return c;}
    static void AddLight(){var g=new GameObject("Key Light");var l=g.AddComponent<Light>();l.type=LightType.Directional;l.intensity=1.5f;l.color=new Color(.7f,.8f,1);g.transform.rotation=Quaternion.Euler(45,-25,0);}
    static GameObject Cube(string n,Vector3 p,Vector3 s,Color c){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.position=p;g.transform.localScale=s;g.GetComponent<Renderer>().sharedMaterial=Mat(c);return g;}
    static Material Mat(Color c){var shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");return new Material(shader){color=c};}
    static Canvas MakeCanvas(){var g=new GameObject("Canvas",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));var c=g.GetComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;var s=g.GetComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1280,720);new GameObject("EventSystem",typeof(UnityEngine.EventSystems.EventSystem),typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));return c;}
    static GameObject Panel(Transform p,string n,Color c){var i=UiImage(p,n,c,Vector2.zero,new Vector2(1280,720));i.rectTransform.anchorMin=Vector2.zero;i.rectTransform.anchorMax=Vector2.one;i.rectTransform.sizeDelta=Vector2.zero;return i.gameObject;}
    static Text Label(Transform p,string n,string value,int size,Vector2 pos,Vector2 dim){var g=new GameObject(n,typeof(RectTransform),typeof(Text));g.transform.SetParent(p,false);var t=g.GetComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.supportRichText=true;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=dim;return t;}
    static Image UiImage(Transform p,string n,Color c,Vector2 pos,Vector2 size){var g=new GameObject(n,typeof(RectTransform),typeof(Image));g.transform.SetParent(p,false);var i=g.GetComponent<Image>();i.color=c;i.rectTransform.anchoredPosition=pos;i.rectTransform.sizeDelta=size;return i;}
    static void MakeButton(Transform p,string text,Vector2 pos,UnityAction action){var i=UiImage(p,text+" Button",new Color(.12f,.18f,.4f,.95f),pos,new Vector2(300,54));var b=i.gameObject.AddComponent<Button>();UnityEventTools.AddPersistentListener(b.onClick,action);Label(i.transform,"Label",text.ToUpperInvariant(),22,Vector2.zero,new Vector2(290,50));}
    static void MakeSlider(Transform p,string label,Vector2 pos,float value,UnityAction<float> action){Label(p,label,label,18,pos+new Vector2(-245,0),new Vector2(220,40));var bg=UiImage(p,label+" Slider",new Color(.1f,.12f,.25f),pos,new Vector2(360,24));var fill=UiImage(bg.transform,"Fill",Color.cyan,Vector2.zero,new Vector2(350,18));var s=bg.gameObject.AddComponent<Slider>();s.fillRect=fill.rectTransform;s.value=value;UnityEventTools.AddPersistentListener(s.onValueChanged,action);}
    static void MakeToggle(Transform p,string label,Vector2 pos,bool value,UnityAction<bool> action){var bg=UiImage(p,label+" Toggle",new Color(.1f,.12f,.25f),pos,new Vector2(360,44));var mark=UiImage(bg.transform,"Checkmark",Color.cyan,new Vector2(-150,0),new Vector2(28,28));var t=bg.gameObject.AddComponent<Toggle>();t.targetGraphic=bg;t.graphic=mark;t.isOn=value;UnityEventTools.AddPersistentListener(t.onValueChanged,action);Label(bg.transform,"Label",label,18,new Vector2(30,0),new Vector2(280,40));}
    static void MakeDropdown(Transform p,string label,Vector2 pos,string[] options,UnityAction<int> action){Label(p,label,label,18,pos+new Vector2(-245,0),new Vector2(220,40));var bg=UiImage(p,label+" Dropdown",new Color(.1f,.12f,.25f),pos,new Vector2(360,44));var caption=Label(bg.transform,"Caption","SELECT",18,Vector2.zero,new Vector2(340,40));var d=bg.gameObject.AddComponent<Dropdown>();d.captionText=caption;foreach(var o in options)d.options.Add(new Dropdown.OptionData(o));UnityEventTools.AddPersistentListener(d.onValueChanged,action);}
    static void Set(Object o,string property,Object value){var so=new SerializedObject(o);so.FindProperty(property).objectReferenceValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
}

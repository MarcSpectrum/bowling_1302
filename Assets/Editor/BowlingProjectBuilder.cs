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
        var menuPath=System.IO.Path.GetFullPath(Scenes+"MainMenu.unity");
        var menuScene=AssetDatabase.LoadAssetAtPath<SceneAsset>(Scenes+"MainMenu.unity");
        if(menuScene!=null)EditorSceneManager.playModeStartScene=menuScene;
        if (!System.IO.File.Exists(menuPath)||!System.IO.File.ReadAllText(menuPath).Contains("Cinematic Backdrop"))
            EditorApplication.delayCall += Build;
    }
    [MenuItem("Bowling 1302/Rebuild Game Scenes")]
    public static void Build()
    {
        font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); BuildMenu(); BuildGame();
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scenes+"MainMenu.unity",true),new EditorBuildSettingsScene(Scenes+"BowlingGame.unity",true)};
        EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>(Scenes+"MainMenu.unity");
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
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single); AddCamera(new Color(.008f,.006f,.004f));
        var canvas=MakeCanvas(); var root=new GameObject("MainMenuController"); var c=root.AddComponent<MainMenuController>();
        var backdrop=new GameObject("Cinematic Backdrop",typeof(RectTransform),typeof(RawImage),typeof(AspectRatioFitter));backdrop.transform.SetParent(canvas.transform,false);
        var br=backdrop.GetComponent<RectTransform>();br.anchorMin=Vector2.zero;br.anchorMax=Vector2.one;br.sizeDelta=Vector2.zero;
        backdrop.GetComponent<RawImage>().texture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/BowlingMenuCover.png");var fitter=backdrop.GetComponent<AspectRatioFitter>();fitter.aspectMode=AspectRatioFitter.AspectMode.EnvelopeParent;fitter.aspectRatio=16f/9f;
        UiImage(canvas.transform,"Left Shadow",new Color(.008f,.006f,.004f,.88f),new Vector2(-384,0),new Vector2(512,720));UiImage(canvas.transform,"Gold Accent",new Color(.88f,.64f,.2f,1),new Vector2(-636,0),new Vector2(8,720));
        var home=MenuPanel(canvas.transform,"Home");
        var eyebrow=Label(home.transform,"Eyebrow","1302 PRESENTS",17,new Vector2(-385,220),new Vector2(390,35));Left(eyebrow,new Color(.78f,.76f,.68f));
        var title=Label(home.transform,"Title","BOWLING",64,new Vector2(-385,150),new Vector2(390,80));Left(title,new Color(1f,.76f,.27f),FontStyle.Bold);
        var tagline=Label(home.transform,"Tagline","THE GOLDEN LANE",19,new Vector2(-385,100),new Vector2(390,35));Left(tagline,new Color(.88f,.82f,.7f));
        var intro=Label(home.transform,"Intro","Ten frames. One perfect game.\nOwn the lane and chase 300.",17,new Vector2(-385,55),new Vector2(390,55));Left(intro,new Color(.76f,.76f,.72f));
        MakeMenuButton(home.transform,"Play Game",new Vector2(-385,-25),c.Play);MakeMenuButton(home.transform,"Settings",new Vector2(-385,-92),c.ShowSettings);MakeMenuButton(home.transform,"Credits",new Vector2(-385,-159),c.ShowCredits);MakeMenuButton(home.transform,"Quit Game",new Vector2(-385,-226),c.Quit);
        var controls=Label(home.transform,"Controls","A / D TO MOVE  •  Q / E TO AIM\nHOLD & RELEASE SPACE TO BOWL  •  ESC TO PAUSE",14,new Vector2(-385,-305),new Vector2(390,55));Left(controls,new Color(.7f,.7f,.66f));
        var settings=MenuPanel(canvas.transform,"Settings");settings.SetActive(false);var sh=Label(settings.transform,"Heading","SETTINGS",48,new Vector2(-385,245),new Vector2(390,70));Left(sh,new Color(1f,.76f,.27f),FontStyle.Bold);
        MakeSlider(settings.transform,"MASTER VOLUME",new Vector2(-385,145),.8f,c.SetMaster);MakeSlider(settings.transform,"MUSIC VOLUME",new Vector2(-385,85),.65f,c.SetMusic);MakeSlider(settings.transform,"SFX VOLUME",new Vector2(-385,25),.85f,c.SetSfx);MakeToggle(settings.transform,"FULLSCREEN",new Vector2(-385,-40),true,c.SetFullscreen);MakeDropdown(settings.transform,"QUALITY",new Vector2(-385,-105),QualitySettings.names,c.SetQuality);
        var rs=Screen.resolutions;var opts=new string[rs.Length];for(int i=0;i<rs.Length;i++)opts[i]=$"{rs[i].width} × {rs[i].height}";MakeDropdown(settings.transform,"RESOLUTION",new Vector2(-385,-170),opts,c.SetResolution);MakeMenuButton(settings.transform,"Back",new Vector2(-385,-265),c.ShowHome);
        var credits=MenuPanel(canvas.transform,"Credits");credits.SetActive(false);var ch=Label(credits.transform,"Heading","CREDITS",48,new Vector2(-385,220),new Vector2(390,70));Left(ch,new Color(1f,.76f,.27f),FontStyle.Bold);
        var copy=Label(credits.transform,"Copy","BOWLING 1302\n\nDesigned and built with Unity.\nArcade framework and original menu art.\n\nWith thanks to the Unity community\nand open-source contributors.",18,new Vector2(-385,40),new Vector2(390,280));Left(copy,new Color(.8f,.79f,.73f));MakeMenuButton(credits.transform,"Back",new Vector2(-385,-200),c.ShowHome);
        Set(c,"home",home);Set(c,"settings",settings);Set(c,"credits",credits);EditorSceneManager.SaveScene(scene,Scenes+"MainMenu.unity");
    }
    static void BuildGame()
    {
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single); RenderSettings.ambientLight=new Color(.12f,.14f,.25f);
        var cam=AddCamera(new Color(.015f,.02f,.07f));cam.transform.position=new Vector3(0,20,-11);cam.transform.LookAt(new Vector3(0,0,9));cam.fieldOfView=58;cam.nearClipPlane=.1f;cam.farClipPlane=80;AddLight();
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
    static GameObject MenuPanel(Transform p,string n){var g=new GameObject(n,typeof(RectTransform),typeof(CanvasGroup));g.transform.SetParent(p,false);var r=g.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.sizeDelta=Vector2.zero;return g;}
    static Text Label(Transform p,string n,string value,int size,Vector2 pos,Vector2 dim){var g=new GameObject(n,typeof(RectTransform),typeof(Text));g.transform.SetParent(p,false);var t=g.GetComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.supportRichText=true;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=dim;return t;}
    static void Left(Text text,Color color,FontStyle style=FontStyle.Normal){text.alignment=TextAnchor.MiddleLeft;text.color=color;text.fontStyle=style;}
    static Image UiImage(Transform p,string n,Color c,Vector2 pos,Vector2 size){var g=new GameObject(n,typeof(RectTransform),typeof(Image));g.transform.SetParent(p,false);var i=g.GetComponent<Image>();i.color=c;i.rectTransform.anchoredPosition=pos;i.rectTransform.sizeDelta=size;return i;}
    static void MakeButton(Transform p,string text,Vector2 pos,UnityAction action){var i=UiImage(p,text+" Button",new Color(.12f,.18f,.4f,.95f),pos,new Vector2(300,54));var b=i.gameObject.AddComponent<Button>();UnityEventTools.AddPersistentListener(b.onClick,action);Label(i.transform,"Label",text.ToUpperInvariant(),22,Vector2.zero,new Vector2(290,50));}
    static void MakeMenuButton(Transform p,string text,Vector2 pos,UnityAction action){var i=UiImage(p,text+" Button",new Color(.08f,.065f,.045f,.88f),pos,new Vector2(390,54));var b=i.gameObject.AddComponent<Button>();var colors=b.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1f,.78f,.34f,1);colors.selectedColor=colors.highlightedColor;colors.pressedColor=new Color(.82f,.58f,.2f,1);colors.fadeDuration=.12f;b.colors=colors;UnityEventTools.AddPersistentListener(b.onClick,action);var t=Label(i.transform,"Label",text.ToUpperInvariant(),21,Vector2.zero,new Vector2(360,50));Left(t,Color.white,FontStyle.Bold);}
    static void MakeSlider(Transform p,string label,Vector2 pos,float value,UnityAction<float> action){var l=Label(p,label,label,14,pos+new Vector2(0,20),new Vector2(390,24));Left(l,new Color(.82f,.79f,.7f));var bg=UiImage(p,label+" Slider",new Color(.12f,.095f,.06f,.95f),pos+new Vector2(0,-13),new Vector2(390,18));var fill=UiImage(bg.transform,"Fill",new Color(.9f,.65f,.2f),Vector2.zero,new Vector2(382,12));var s=bg.gameObject.AddComponent<Slider>();s.fillRect=fill.rectTransform;s.value=value;UnityEventTools.AddPersistentListener(s.onValueChanged,action);}
    static void MakeToggle(Transform p,string label,Vector2 pos,bool value,UnityAction<bool> action){var bg=UiImage(p,label+" Toggle",new Color(.08f,.065f,.045f,.88f),pos,new Vector2(390,44));var mark=UiImage(bg.transform,"Checkmark",new Color(.9f,.65f,.2f),new Vector2(-165,0),new Vector2(24,24));var t=bg.gameObject.AddComponent<Toggle>();t.targetGraphic=bg;t.graphic=mark;t.isOn=value;UnityEventTools.AddPersistentListener(t.onValueChanged,action);var l=Label(bg.transform,"Label",label,16,new Vector2(20,0),new Vector2(320,40));Left(l,new Color(.9f,.87f,.78f));}
    static void MakeDropdown(Transform p,string label,Vector2 pos,string[] options,UnityAction<int> action)
    {
        var l=Label(p,label,label,14,pos+new Vector2(0,28),new Vector2(390,22));Left(l,new Color(.82f,.79f,.7f));var bg=UiImage(p,label+" Dropdown",new Color(.08f,.065f,.045f,.96f),pos,new Vector2(390,40));
        var caption=Label(bg.transform,"Caption",options.Length>0?options[0]:"NOT AVAILABLE",16,Vector2.zero,new Vector2(350,36));Left(caption,new Color(.94f,.9f,.8f));var d=bg.gameObject.AddComponent<Dropdown>();d.captionText=caption;
        var template=UiImage(bg.transform,"Template",new Color(.045f,.035f,.025f,.99f),new Vector2(0,-125),new Vector2(390,200));var scroll=template.gameObject.AddComponent<ScrollRect>();
        var viewport=UiImage(template.transform,"Viewport",Color.white,Vector2.zero,new Vector2(382,192));viewport.gameObject.AddComponent<Mask>().showMaskGraphic=false;var content=new GameObject("Content",typeof(RectTransform));content.transform.SetParent(viewport.transform,false);var cr=content.GetComponent<RectTransform>();cr.anchorMin=new Vector2(0,1);cr.anchorMax=Vector2.one;cr.pivot=new Vector2(.5f,1);cr.sizeDelta=new Vector2(0,Mathf.Max(36,options.Length*36));
        var item=UiImage(content.transform,"Item Background",new Color(.08f,.065f,.045f,1),new Vector2(0,-18),new Vector2(374,34));var toggle=item.gameObject.AddComponent<Toggle>();var itemText=Label(item.transform,"Item Label","OPTION",15,Vector2.zero,new Vector2(340,32));Left(itemText,new Color(.94f,.9f,.8f));
        scroll.viewport=viewport.rectTransform;scroll.content=cr;scroll.horizontal=false;d.template=template.rectTransform;d.itemText=itemText;template.gameObject.SetActive(false);
        foreach(var o in options)d.options.Add(new Dropdown.OptionData(o));UnityEventTools.AddPersistentListener(d.onValueChanged,action);
    }
    static void Set(Object o,string property,Object value){var so=new SerializedObject(o);so.FindProperty(property).objectReferenceValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CharacterAnimationTester : EditorWindow
{
    private GameObject selectedCharacter;
    private GameObject previewCharacter;

    private AnimationClip[] animationClips;
    private int selectedAnimation = 0;

    private bool isPlaying = false;
    private bool loop = true;

    private float animationTime = 0f;
    private float playbackSpeed = 1f;

    private PreviewRenderUtility previewUtility;

    private Vector2 scrollPosition;

    private float rotation = 0f;
    private float zoom = 5f;

    private double lastTime;


    [MenuItem("Tools/Character Animation Tester")]
    public static void ShowWindow()
    {
        CharacterAnimationTester window =
            GetWindow<CharacterAnimationTester>();

        window.titleContent =
            new GUIContent("Animation Tester");

        window.minSize =
            new Vector2(500, 600);
    }


    private void OnEnable()
    {
        previewUtility =
            new PreviewRenderUtility();

        previewUtility.cameraFieldOfView = 30f;

        previewUtility.camera.transform.position =
            new Vector3(0, 1.5f, -5f);

        previewUtility.camera.transform.LookAt(
            Vector3.up * 1.2f
        );

        lastTime =
            EditorApplication.timeSinceStartup;

        EditorApplication.update += UpdatePreview;

        AnimationMode.StartAnimationMode();
    }


    private void OnDisable()
    {
        EditorApplication.update -= UpdatePreview;

        if (AnimationMode.InAnimationMode())
        {
            AnimationMode.StopAnimationMode();
        }

        DestroyPreviewCharacter();

        if (previewUtility != null)
        {
            previewUtility.Cleanup();
            previewUtility = null;
        }
    }


    private void OnGUI()
    {
        scrollPosition =
            EditorGUILayout.BeginScrollView(
                scrollPosition
            );

        GUILayout.Space(10);

        GUILayout.Label(
            "Character Animation Tester",
            EditorStyles.boldLabel
        );

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Select an FBX or character prefab to preview its animations.",
            MessageType.Info
        );

        GUILayout.Space(10);


        // CHARACTER

        EditorGUI.BeginChangeCheck();

        selectedCharacter =
            (GameObject)EditorGUILayout.ObjectField(
                "Character",
                selectedCharacter,
                typeof(GameObject),
                false
            );

        if (EditorGUI.EndChangeCheck())
        {
            LoadCharacter();
        }


        GUILayout.Space(10);


        // ANIMATIONS

        if (animationClips != null &&
            animationClips.Length > 0)
        {
            string[] animationNames =
                new string[animationClips.Length];

            for (int i = 0;
                i < animationClips.Length;
                i++)
            {
                animationNames[i] =
                    animationClips[i].name;
            }


            EditorGUI.BeginChangeCheck();

            selectedAnimation =
                EditorGUILayout.Popup(
                    "Animation",
                    selectedAnimation,
                    animationNames
                );

            if (EditorGUI.EndChangeCheck())
            {
                animationTime = 0f;

                isPlaying = false;

                SampleAnimation();
            }


            GUILayout.Space(10);


            AnimationClip currentClip =
                animationClips[selectedAnimation];


            // PLAY / PAUSE / STOP

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(
                isPlaying ? "Pause" : "Play",
                GUILayout.Height(30)))
            {
                isPlaying = !isPlaying;

                lastTime =
                    EditorApplication.timeSinceStartup;
            }


            if (GUILayout.Button(
                "Stop",
                GUILayout.Height(30)))
            {
                isPlaying = false;

                animationTime = 0f;

                SampleAnimation();
            }

            EditorGUILayout.EndHorizontal();


            GUILayout.Space(10);


            // ANIMATION TIME

            EditorGUILayout.LabelField(
                "Animation Time"
            );

            float newTime =
                EditorGUILayout.Slider(
                    animationTime,
                    0f,
                    Mathf.Max(
                        currentClip.length,
                        0.01f
                    )
                );


            if (!Mathf.Approximately(
                newTime,
                animationTime))
            {
                animationTime = newTime;

                isPlaying = false;

                SampleAnimation();
            }


            GUILayout.Space(5);


            EditorGUILayout.LabelField(
                "Time: " +
                animationTime.ToString("0.00") +
                " / " +
                currentClip.length.ToString("0.00") +
                " seconds"
            );


            GUILayout.Space(10);


            // SPEED

            playbackSpeed =
                EditorGUILayout.Slider(
                    "Playback Speed",
                    playbackSpeed,
                    0.1f,
                    3f
                );


            // LOOP

            loop =
                EditorGUILayout.Toggle(
                    "Loop",
                    loop
                );


            GUILayout.Space(10);


            // ROTATION

            rotation =
                EditorGUILayout.Slider(
                    "Character Rotation",
                    rotation,
                    0f,
                    360f
                );


            if (previewCharacter != null)
            {
                previewCharacter.transform.rotation =
                    Quaternion.Euler(
                        0f,
                        rotation,
                        0f
                    );
            }


            // ZOOM

            zoom =
                EditorGUILayout.Slider(
                    "Camera Zoom",
                    zoom,
                    1f,
                    10f
                );


            GUILayout.Space(10);


            // INFORMATION

            EditorGUILayout.LabelField(
                "Animation Information",
                EditorStyles.boldLabel
            );


            EditorGUILayout.LabelField(
                "Name",
                currentClip.name
            );


            EditorGUILayout.LabelField(
                "Length",
                currentClip.length.ToString("0.00") +
                " seconds"
            );


            EditorGUILayout.LabelField(
                "Frame Rate",
                currentClip.frameRate.ToString("0.00") +
                " FPS"
            );
        }
        else if (selectedCharacter != null)
        {
            EditorGUILayout.HelpBox(
                "No animation clips were found on this asset.",
                MessageType.Warning
            );
        }


        GUILayout.Space(20);


        // PREVIEW

        if (previewUtility != null &&
            previewCharacter != null)
        {
            Rect previewRect =
                GUILayoutUtility.GetRect(
                    400,
                    400,
                    GUILayout.ExpandWidth(true)
                );

            DrawPreview(previewRect);
        }


        EditorGUILayout.EndScrollView();
    }


    private void LoadCharacter()
    {
        DestroyPreviewCharacter();

        animationClips = null;

        selectedAnimation = 0;

        animationTime = 0f;

        isPlaying = false;


        if (selectedCharacter == null)
        {
            Repaint();

            return;
        }


        // CREATE PREVIEW COPY

        previewCharacter =
            Instantiate(
                selectedCharacter
            );

        previewCharacter.name =
            selectedCharacter.name +
            "_Preview";

        previewCharacter.hideFlags =
            HideFlags.HideAndDontSave;


        // DISABLE SCRIPTS

        MonoBehaviour[] scripts =
            previewCharacter
                .GetComponentsInChildren<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }


        previewUtility.AddSingleGO(
            previewCharacter
        );


        // FIND ANIMATIONS

        string assetPath =
            AssetDatabase.GetAssetPath(
                selectedCharacter
            );


        if (!string.IsNullOrEmpty(assetPath))
        {
            Object[] assets =
                AssetDatabase.LoadAllAssetsAtPath(
                    assetPath
                );

            List<AnimationClip> clips =
                new List<AnimationClip>();


            foreach (Object asset in assets)
            {
                AnimationClip clip =
                    asset as AnimationClip;


                if (clip != null &&
                    !clip.name.StartsWith(
                        "__preview__"))
                {
                    clips.Add(clip);
                }
            }


            animationClips =
                clips.ToArray();
        }


        FrameCamera();


        if (animationClips != null &&
            animationClips.Length > 0)
        {
            SampleAnimation();
        }


        Repaint();
    }


    private void DestroyPreviewCharacter()
    {
        if (previewCharacter != null)
        {
            DestroyImmediate(
                previewCharacter
            );

            previewCharacter = null;
        }
    }


    private void SampleAnimation()
    {
        if (previewCharacter == null ||
            animationClips == null ||
            animationClips.Length == 0)
        {
            return;
        }


        AnimationClip clip =
            animationClips[selectedAnimation];


        animationTime =
            Mathf.Clamp(
                animationTime,
                0f,
                clip.length
            );


        if (!AnimationMode.InAnimationMode())
        {
            AnimationMode.StartAnimationMode();
        }


        AnimationMode.SampleAnimationClip(
            previewCharacter,
            clip,
            animationTime
        );


        Repaint();
    }


    private void UpdatePreview()
    {
        if (!isPlaying ||
            animationClips == null ||
            animationClips.Length == 0)
        {
            lastTime =
                EditorApplication.timeSinceStartup;

            return;
        }


        double currentTime =
            EditorApplication.timeSinceStartup;


        float deltaTime =
            (float)(
                currentTime -
                lastTime
            );


        lastTime = currentTime;


        AnimationClip clip =
            animationClips[selectedAnimation];


        animationTime +=
            deltaTime *
            playbackSpeed;


        if (animationTime >= clip.length)
        {
            if (loop)
            {
                animationTime %= clip.length;
            }
            else
            {
                animationTime = clip.length;

                isPlaying = false;
            }
        }


        SampleAnimation();

        Repaint();
    }


    private void FrameCamera()
    {
        if (previewCharacter == null)
            return;


        Renderer[] renderers =
            previewCharacter
                .GetComponentsInChildren<Renderer>();


        if (renderers.Length == 0)
            return;


        Bounds bounds =
            renderers[0].bounds;


        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(
                renderer.bounds
            );
        }


        Vector3 center =
            bounds.center;


        float size =
            Mathf.Max(
                bounds.size.x,
                bounds.size.y,
                bounds.size.z
            );


        previewUtility.camera.transform.position =
            center +
            new Vector3(
                0,
                size * 0.1f,
                -size * 2.5f
            );


        previewUtility.camera.transform.LookAt(
            center
        );
    }


    private void DrawPreview(Rect rect)
    {
        if (previewUtility == null)
            return;


        Camera camera =
            previewUtility.camera;


        camera.transform.position =
            new Vector3(
                0,
                1.2f,
                -zoom
            );


        camera.transform.LookAt(
            Vector3.up * 1.2f
        );


        previewUtility.lights[0].intensity =
            1.2f;


        previewUtility.lights[0]
            .transform.rotation =
            Quaternion.Euler(
                30f,
                -30f,
                0f
            );


        previewUtility.lights[1].intensity =
            0.6f;


        previewUtility.BeginPreview(
            rect,
            GUIStyle.none
        );


        previewUtility.camera.Render();


        Texture result =
            previewUtility.EndPreview();


        GUI.DrawTexture(
            rect,
            result,
            ScaleMode.StretchToFill,
            false
        );
    }
}
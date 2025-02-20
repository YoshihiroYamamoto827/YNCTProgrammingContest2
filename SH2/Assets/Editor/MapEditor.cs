using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;


//jsonに出力する配列
[System.Serializable]
public class Jsondata
{
    public Mapdata[] mapdata; 
}

//jsonデータのフォーマット
[System.Serializable]
public class Mapdata
{
    public int xcoor;
    public int ycoor;
    public string objectname;
}

//マップサイズ等の情報
[System.Serializable]
public class MapInfo
{
    public int mapsize;
    public string date;
}

[System.Serializable]
public class GimmicInfo
{
    public Gimmicdata[] gimmicdata;
}

//jsonデータのフォーマット
[System.Serializable]
public class Gimmicdata
{
    bool exit;
    int Xmovecoor;
    int Ymovecoor;
}



//MapEditor
public class MapEditor : EditorWindow
{
    //画像ディレクトリ
    private Object imgDirectory;
    //出力先ディレクトリ(未指定の場合Asset下に出力)
    private Object outputDirectory;
    //マップエディタのマスの数
    private int mapSize = 10;
    //グリッドの大きさ
    private int gridSize = 50;
    //出力フォルダ名
    private string outputFolderName;
    //選択した画像のパス
    private string selectedImagePath;
    //サブウィンドウ
    private MapEditorSubWindow subWindow;
    //private MapEditorSubWindow2 subWindow2;

    [UnityEditor.MenuItem("Window/MapEditor")]
    static void ShowTestMainWindow()
    {
        EditorWindow.GetWindow(typeof(MapEditor));
    }

    private void OnGUI()
    {
        //GUI上で表示
        GUILayout.BeginHorizontal();
        GUILayout.Label("Image Directory:", GUILayout.Width(150));
        imgDirectory = EditorGUILayout.ObjectField(imgDirectory, typeof(UnityEngine.Object), true);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Map Size:", GUILayout.Width(150));
        mapSize = EditorGUILayout.IntField(mapSize);
        if (mapSize > 100)
        {
            mapSize = 100;
        }
        else
        {
         if (mapSize < 5) mapSize = 5;
        }
        
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Size:", GUILayout.Width(150));
        gridSize = EditorGUILayout.IntField(gridSize);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Save Directory:", GUILayout.Width(150));
        outputDirectory = EditorGUILayout.ObjectField(outputDirectory, typeof(UnityEngine.Object), true);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Save DirectoryName:", GUILayout.Width(150));
        outputFolderName = (string)EditorGUILayout.TextField(outputFolderName);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        using (new GUILayout.VerticalScope())
        {
            DrawImageParts();
            DrawSelectedImage();
            DrawMapWindowButton();
            GUILayout.Space(20);
            DrawOutputButton();
        }
    }

    //画像一覧をボタンとして出力
    private void DrawImageParts()
    {
        if (imgDirectory != null)
        {
            float x = 0.0f;
            float y = 00.0f;
            float w = 50.0f;
            float h = 50.0f;
            float maxW = 300.0f;

            string path = AssetDatabase.GetAssetPath(imgDirectory);
            string[] names = Directory.GetFiles(path, "*.png");
            EditorGUILayout.BeginVertical();
            foreach (string d in names)
            {
                if (x > maxW)
                {
                    x = 0.0f;
                    y += h;
                    EditorGUILayout.EndHorizontal();
                }

                if (x == 0.0f)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                GUILayout.FlexibleSpace();
                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(d, typeof(Texture2D));
                if (GUILayout.Button(tex, GUILayout.MaxWidth(w), GUILayout.MaxHeight(h), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
                {
                    selectedImagePath = d;
                }
                GUILayout.FlexibleSpace();
                x += w;
            }
            EditorGUILayout.EndVertical();
        }
    }

    //選択した画像データを表示
    private void DrawSelectedImage()
    {
        if (selectedImagePath != null)
        {
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(selectedImagePath, typeof(Texture2D));
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("select:" + selectedImagePath);
            GUILayout.Box(tex);
            EditorGUILayout.EndVertical();
        }
    }

    //マップウィンドウを開くボタンを生成
    private void DrawMapWindowButton()
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("open map editor", GUILayout.MinWidth(300), GUILayout.MinHeight(50)))
        {
            if (subWindow == null)
            {
                subWindow = MapEditorSubWindow.WillAppear(this);
                //subWindow2 = MapEditorSubWindow2.WillAppear(this);
            }
            else
            {
                subWindow.Focus();
            }
        }
    }

    private void DrawOutputButton()
    {
        //出力ボタン
        if (GUILayout.Button("output file", GUILayout.MinWidth(300), GUILayout.MinHeight(50)))
        {
            if(subWindow == null)
            {
                subWindow = MapEditorSubWindow.WillAppear(this);
            }
            else
            {
               subWindow.OutputFile();
            }
        }
    }

    public string SelectedImagePath
    {
        get { return selectedImagePath; }
    }

    public int MapSize
    {
        get { return mapSize; }
    }

    public int GridSize
    {
        get { return gridSize; }
    }

    //出力先パスを生成
    public string OutputFilePath()
    {
        string resultPath = "";
        if (outputDirectory != null)
        {
            if (System.IO.Directory.Exists(AssetDatabase.GetAssetPath(outputDirectory) + "/" + outputFolderName) == false)
            {
                Debug.Log("作成");
                System.IO.Directory.CreateDirectory(AssetDatabase.GetAssetPath(outputDirectory) + "/" + outputFolderName);
            }

            resultPath = AssetDatabase.GetAssetPath(outputDirectory);
        }
        else
        {
            resultPath = Application.dataPath;
        }
        return resultPath + "/" + outputFolderName;
    }
}

//MapEditor SubWindow
public class MapEditorSubWindow : EditorWindow
{
    //マップウィンドウのサイズ
    const float WINDOW_W = 750.0f;
    const float WINDOW_H = 750.0f;
    //マップのグリッド数
    private int mapSize = 0;
    //グリッドサイズ
    private int gridSize = 0;
    //マップデータ
    private string[,] map;
    //マップのマスのTexture
    private Texture _texture;
    //マップ表示エリアの余白
    private int Areamargin;
    //マスの大きさ
    private Rect[] MapRects; 
    //マップ生成時のマスのx座標とy座標
    private int measureX, measureY;
    //親ウィンドウの参照
    private MapEditor parent;
    //スクロール位置を記録
    private Vector2 scrollPos = Vector2.zero;
    //スクロールの程度を検知するための変数
    private Vector2 scrollmonitor = Vector2.zero;

    Jsondata json = new Jsondata();
    MapInfo info = new MapInfo();


    //書き込むjsonデータの文字列の定義
    public string jsonstr;
    public string mapinfostr;

    //サブウィンドウを開く
    public static MapEditorSubWindow WillAppear(MapEditor _parent)
    {
        MapEditorSubWindow window = (MapEditorSubWindow)EditorWindow.GetWindow(typeof(MapEditorSubWindow), false);
        window.Show();
        window.minSize = new Vector2(WINDOW_W, WINDOW_H);
        window.SetParent(_parent);
        window.init();
        return window;
    }

    private void SetParent(MapEditor _parent)
    {
        parent = _parent;
    }

    //サブウィンドウの初期化
    public void init()
    {
        mapSize = parent.MapSize;
        Debug.Log(mapSize);
        gridSize = parent.GridSize;
        Areamargin = 10;

        json.mapdata = new Mapdata[mapSize * mapSize];

        //マップデータ、書き込むJsonデータの配列を初期化
        map = new string[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                json.mapdata[i * mapSize + j] = new Mapdata();
                map[i, j] = "";
            }
        }

        //Mapのマスを描画するRectsとTextureの初期化
        MapRects = new Rect[mapSize * mapSize];
        var measureTexture = new Texture2D(1, 1);
        measureTexture.SetPixel(0, 0, Color.white);
        measureTexture.Apply();
        _texture = measureTexture;

        

    }

    void OnGUI()
    {
        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
        {
            scrollPos = scrollView.scrollPosition;

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                        //グリッド線を描画する
                        for (int xx = 0; xx < mapSize; xx++)
                        {
                            for (int yy = 0; yy < mapSize; yy++)
                            {
                                measureX = Areamargin * 3 + gridSize * xx;
                                measureY = Areamargin * 3 + gridSize * yy;
                                MapRects[(xx * mapSize) + yy] = new Rect(measureX, measureY, gridSize, gridSize);
                                GUI.DrawTexture(MapRects[(xx * mapSize) + yy], _texture, ScaleMode.StretchToFill, true, 0, Color.white, 3, 0);
                            }
                        }

                    //クリックされた位置を探してその場所に画像データを入れる
                    Event e = Event.current;
                    if (e.type == EventType.MouseDown)
                    {
                        Vector2 pos = Event.current.mousePosition;
                        int xx;
                        bool xmax = false;

                        //x位置を探す
                        for (xx = 0; xx < (mapSize - 1); xx++)
                        {
                            if (MapRects[(xx * mapSize)].x <= pos.x && pos.x <= MapRects[((xx + 1) * mapSize)].x)
                            {

                                Debug.Log(xx);
                                break;
                            }

                            if (xx == mapSize - 2) xmax = true;
                        }

                        if (xmax && xx == (mapSize - 2)) xx = mapSize - 1;

                        //y位置を探す
                        for (int yy = 0; yy < (mapSize - 1); yy++)
                        {
                            if (MapRects[yy].y <= pos.y && pos.y <= MapRects[(yy + 1)].y)
                            {
                                //消しゴムのときはデータを消す
                                if (parent.SelectedImagePath.IndexOf("000") > -1)
                                {
                                    map[xx, yy] = "";
                                }
                                else
                                {
                                    map[xx, yy] = parent.SelectedImagePath;
                                }
                                Repaint();
                                break;
                            }

                            if (yy == mapSize - 2)
                            {
                                yy = mapSize - 1;
                                if (parent.SelectedImagePath.IndexOf("000") > -1)
                                {
                                    map[xx, yy] = "";
                                }
                                else
                                {
                                    map[xx, yy] = parent.SelectedImagePath;
                                }
                                Repaint();
                                break;
                            }
                        }
                    }

                    //選択した画像を描画する
                    for (int xx = 0; xx < mapSize; xx++)
                    {
                        for (int yy = 0; yy < mapSize; yy++)
                        {
                            if (map[xx, yy] != null && map[xx, yy].Length > 0)
                            {
                                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(map[xx, yy], typeof(Texture2D));
                                GUI.DrawTexture(MapRects[(xx * mapSize) + yy], tex);
                            }
                        }
                    }

                    //Ctrl + スクロールでマスのサイズを拡大、縮小
                    if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl)
                    {
                        if (scrollPos.y > scrollmonitor.y)
                        {
                            gridSize -= 5;
                            Repaint();
                            scrollmonitor = scrollPos;
                        }

                        if (scrollPos.y < scrollmonitor.y)
                        {
                            gridSize += 5;
                            Repaint();
                            scrollmonitor = scrollPos;
                        }
                    }


                    GUILayout.Space(measureY);

                }

                GUILayout.Space(measureX);
            } 
        } 
    }

    //ファイルで出力
    public void OutputFile()
    {
        string folderpath = parent.OutputFilePath();

        //作成するフォルダ名がnullの場合警告を表示
        if (folderpath == null)
        {
            EditorUtility.DisplayDialog("MapEditor", "作成するフォルダ名を入力してください", "OK");
        }
        else
        {
            FileInfo MDfileInfo = new FileInfo(folderpath + "/" + "Mapdata.json");
            StreamWriter mdsw = MDfileInfo.AppendText();
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    GetMapStrFormat(i, j);
                }
            }
            mdsw.WriteLine(WriteJsonMapData());
            mdsw.Flush();
            mdsw.Close();

            FileInfo MIfileInfo = new FileInfo(folderpath + "/" + "Mapinfo.json");
            StreamWriter misw = MIfileInfo.AppendText();
            GetMapInfoFormat();
            misw.WriteLine(WriteJsonMapInfo());
            misw.Flush();
            misw.Close();

            //完了ポップアップ
            EditorUtility.DisplayDialog("MapEditor", "output file success\n" + folderpath, "OK");
        }

       
    }

    //出力するマップデータ整形
    private void GetMapStrFormat(int x, int y)
    {
        json.mapdata[x * mapSize + y].xcoor = x;
        json.mapdata[x * mapSize + y].ycoor = y;
        json.mapdata[x * mapSize + y].objectname = OutputDataFormat(map[x, y]);
    }

    private void GetMapInfoFormat()
    {
        info.mapsize = mapSize;
        info.date = System.DateTime.Now.Date.ToString();
    }

    private string WriteJsonMapData()
    {
        jsonstr = JsonUtility.ToJson(json, true);
        return jsonstr;
    }

    private string WriteJsonMapInfo()
    {
        mapinfostr = JsonUtility.ToJson(info, true);
        return mapinfostr;
    }

    private string OutputDataFormat(string data)
    {
        if (data != null && data.Length > 0)
        {
            string[] tmps = data.Split('/');
            string fileName = tmps[tmps.Length - 1];
            return fileName.Split('/')[0];
        }
        else
        {
            return "";
        }
    }
}

/*//MapEditor SubWindow2
public class MapEditorSubWindow2 : EditorWindow
{
    //マップウィンドウのサイズ
    const float WINDOW_W = 750.0f;
    const float WINDOW_H = 750.0f;
    //マップのグリッド数
    private int mapSize = 0;
    //グリッドサイズ
    private int gridSize = 0;
    //マップデータ
    private string[,] map;
    //親ウィンドウの参照
    private MapEditor parent;
    //スクロール位置を記録
    private Vector2 scrollPos = Vector2.zero;
    //スクロールの程度を検知するための変数
    private Vector2 scrollmonitor = Vector2.zero;

    Gimmicdata gimmic = new Gimmicdata();


    //書き込むjsonデータの文字列の定義
    public string gimmicstr;

    //サブウィンドウを開く
    public static MapEditorSubWindow2 WillAppear(MapEditor _parent)
    {
        MapEditorSubWindow2 window = (MapEditorSubWindow2)EditorWindow.GetWindow(typeof(MapEditorSubWindow2), false);
        window.Show();
        window.minSize = new Vector2(WINDOW_W, WINDOW_H);
        window.SetParent(_parent);
        window.init();
        return window;
    }

    private void SetParent(MapEditor _parent)
    {
        parent = _parent;
    }

    //サブウィンドウの初期化
    public void init()
    {
        mapSize = parent.MapSize;
        Debug.Log(mapSize);
        gridSize = parent.GridSize;

        json.mapdata = new Mapdata[mapSize * mapSize];

        //マップデータ、書き込むJsonデータの配列を初期化
        map = new string[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                json.mapdata[i * mapSize + j] = new Mapdata();
                map[i, j] = "";
            }
        }
    }

    void OnGUI()
    {
        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
        {
            scrollPos = scrollView.scrollPosition;

            switch(map[x,y])

        }
    }

    //ファイルで出力
    private void OutputFile()
    {

        string folderpath = parent.OutputFilePath();

        FileInfo MDfileInfo = new FileInfo(folderpath + "/" + "Mapdata.json");
        StreamWriter mdsw = MDfileInfo.AppendText();
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                GetMapStrFormat(i, j);
            }
        }
        mdsw.WriteLine(WriteJsonMapData());
        mdsw.Flush();
        mdsw.Close();

        FileInfo MIfileInfo = new FileInfo(folderpath + "/" + "Mapinfo.json");
        StreamWriter misw = MIfileInfo.AppendText();
        GetMapInfoFormat();
        misw.WriteLine(WriteJsonMapInfo());
        misw.Flush();
        misw.Close();

        //完了ポップアップ
        EditorUtility.DisplayDialog("MapEditor", "output file success\n" + folderpath, "OK");
    }

    //出力するマップデータ整形
    private void GetMapStrFormat(int x, int y)
    {
        json.mapdata[x * mapSize + y].xcoor = x;
        json.mapdata[x * mapSize + y].ycoor = y;
        json.mapdata[x * mapSize + y].objectname = OutputDataFormat(map[x, y]);
    }

    private void GetMapInfoFormat()
    {
        info.mapsize = mapSize;
        info.date = System.DateTime.Now.Date.ToString();
    }

    private string WriteJsonMapData()
    {
        jsonstr = JsonUtility.ToJson(json, true);
        return jsonstr;
    }

    private string WriteJsonMapInfo()
    {
        mapinfostr = JsonUtility.ToJson(info, true);
        return mapinfostr;
    }

    private string OutputDataFormat(string data)
    {
        if (data != null && data.Length > 0)
        {
            string[] tmps = data.Split('/');
            string fileName = tmps[tmps.Length - 1];
            return fileName.Split('/')[0];
        }
        else
        {
            return "";
        }
    }
}*/



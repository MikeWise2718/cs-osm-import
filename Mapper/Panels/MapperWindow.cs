using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using System.IO;
using System.Net;
using ColossalFramework.Importers;
using System.Collections;
using Mapper.OSM;
using ColossalFramework.Plugins;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Mapper
{
    public class MapperWindow7 : UIPanel
    {
        UILabel title;

        UITextField pathTextBox;
        UILabel pathTextBoxLabel;
        UIButton loadOsmFromFile;

        UITextField coordinates;
        UILabel coordinatesLabel;
        UIButton loadAPIButton;
        UIButton loadTerrainParty;

        UIButton pedestriansCheck;
        UILabel pedestrianLabel;

        UIButton roadsCheck;
        UILabel roadsLabel;

        UIButton highwaysCheck;
        UILabel highwaysLabel;

        UITextField scaleTextBox;
        UILabel scaleTextBoxLabel;

        UITextField tolerance;
        UILabel toleranceLabel;

        UITextField curveTolerance;
        UILabel curveToleranceLabel;

        UITextField tiles;
        UILabel tilesLabel;

        UILabel errorLabel;
        UILabel infoLabel;
        UILabel nodesXmlInfoLabel;
        UILabel waysXmlInfoLabel;
        UILabel roadMakerInfoLabel;
        UILabel osmInfoLabel;
        UILabel lastHeightMap16InfoLabel;

        UIButton makeRoadsButton;

        private UITextField mapquestKey;
        private UILabel mapquestKeyLabel;

        public ICities.LoadMode mode;
        RoadMaker2 roadMaker;
        private byte[] nodesXml;
        private byte[] waysXml;
        private osmBounds ob;

		bool nodesLoaded = false;
		bool waysLoaded = false;

        bool createRoads;
        int currentIndex = 0;
        bool peds = true;
        bool roads = true;
        bool highways = true;
        private byte[] m_LastHeightmap16;

        // Controls created
        public override void Awake()
        {
            this.isInteractive = true;
            this.enabled = true;

            width = 500;

            title = AddUIComponent<UILabel>();

            coordinates = AddUIComponent<UITextField>();
            coordinatesLabel = AddUIComponent<UILabel>();
            loadAPIButton = AddUIComponent<UIButton>();
            loadTerrainParty = AddUIComponent<UIButton>();

            pathTextBox = AddUIComponent<UITextField>();
            pathTextBoxLabel = AddUIComponent<UILabel>();
            loadOsmFromFile = AddUIComponent<UIButton>();

            pedestriansCheck = AddUIComponent<UIButton>();
            pedestrianLabel = AddUIComponent<UILabel>();
            roadsCheck = AddUIComponent<UIButton>();
            roadsLabel = AddUIComponent<UILabel>();
            highwaysCheck = AddUIComponent<UIButton>();
            highwaysLabel = AddUIComponent<UILabel>();

            scaleTextBox = AddUIComponent<UITextField>();
            scaleTextBoxLabel = AddUIComponent<UILabel>();


            tolerance = AddUIComponent<UITextField>();
            toleranceLabel = AddUIComponent<UILabel>();

            curveTolerance = AddUIComponent<UITextField>();
            curveToleranceLabel = AddUIComponent<UILabel>();

            tiles = AddUIComponent<UITextField>();
            tilesLabel = AddUIComponent<UILabel>();

            mapquestKey = AddUIComponent<UITextField>();
            mapquestKeyLabel = AddUIComponent<UILabel>();


            infoLabel = AddUIComponent<UILabel>();
            nodesXmlInfoLabel = AddUIComponent<UILabel>();
            waysXmlInfoLabel = AddUIComponent<UILabel>();
            roadMakerInfoLabel = AddUIComponent<UILabel>();
            osmInfoLabel = AddUIComponent<UILabel>();
            lastHeightMap16InfoLabel = AddUIComponent<UILabel>();

            errorLabel = AddUIComponent<UILabel>();

            makeRoadsButton = AddUIComponent<UIButton>();

            base.Awake();
        }

        // Controls iniitialized
        public override void Start()
        {
            base.Start();

            relativePosition = new Vector3(396, 58);
            backgroundSprite = "MenuPanel2";
            isInteractive = true;
            //this.CenterToParent();
            SetupControls();
        }

        // Called by start
        public void SetupControls()
        {
            title.text = "Mike's OSM Import 1";
            title.relativePosition = new Vector3(15, 15);
            title.textScale = 0.9f;
            title.size = new Vector2(200, 30);
            var vertPadding = 30;
            var x = 15;
            var y = 50;

            SetLabel(pedestrianLabel, "Pedestrian Paths", x, y);
            SetButton(pedestriansCheck, "True", x + 114, y);
            pedestriansCheck.eventClick += pedestriansCheck_eventClick;
            x += 190;
            SetLabel(roadsLabel, "Roads", x, y);
            SetButton(roadsCheck, "True", x + 80, y);
            roadsCheck.eventClick += roadsCheck_eventClick;
            x += 140;
            SetLabel(highwaysLabel, "Highways", x, y);
            SetButton(highwaysCheck, "True", x + 80, y);
            highwaysCheck.eventClick += highwaysCheck_eventClick;

            x = 15;
            y += vertPadding;

            SetLabel(scaleTextBoxLabel, "Scale", x, y);
            SetTextBox(scaleTextBox, "1", x + 120, y);
            y += vertPadding;


            SetLabel(toleranceLabel, "Tolerance", x, y);
            SetTextBox(tolerance, "6", x + 120, y);
            y += vertPadding;

            SetLabel(curveToleranceLabel, "Curve Tolerance", x, y);
            SetTextBox(curveTolerance, "6", x + 120, y);
            y += vertPadding;

            SetLabel(tilesLabel, "Tiles to Boundary", x, y);
            SetTextBox(tiles, "2.5", x + 120, y);
            y += vertPadding + 12;

            SetLabel(pathTextBoxLabel, "Path", x, y);
            SetTextBox(pathTextBox,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "osmmaps\\map.osm"), x + 120, y);
            y += vertPadding - 5;
            SetButton(loadOsmFromFile, "Load OSM From File", y);
            loadOsmFromFile.eventClick += loadOsmFromFileButton_eventClick;
            y += vertPadding + 5;

            SetLabel(coordinatesLabel, "Bounding Box", x, y);
            //SetTextBox(coordinates, "35.949310,34.522050,35.753054,34.360353", x + 120, y);
            //SetTextBox(coordinates, "-122.2574,47.5688,-122.0170,47.7305", x + 120, y);
            SetTextBox(coordinates, "-122.1436,47.6317,-122.1208,47.6463", x + 120, y);
            y += vertPadding - 5;

            SetButton(loadTerrainParty, "Load From terrain.party", y);
            loadTerrainParty.tooltip = "Load terrain height data from terrain.party.";
            loadTerrainParty.eventClick += loadTerrainParty_eventClick;
            y += vertPadding + 5;

            SetButton(loadAPIButton, "Load From OpenStreetMap", y);
            loadAPIButton.tooltip = "Load road map data from OpenStreetMap";
            loadAPIButton.eventClick += loadAPIButton_eventClick;
            y += vertPadding + 5;

            SetLabel(infoLabel, "InfoLabel", x, y);
            infoLabel.textScale = 0.6f;
            y += vertPadding/2 + 2;

            SetLabel(lastHeightMap16InfoLabel, "lastHeightMap16InfoLabel", x, y);
            lastHeightMap16InfoLabel.textScale = 0.6f;
            y += vertPadding/2 + 2;

            SetLabel(nodesXmlInfoLabel, "nodesXmlInfoLabel", x, y);
            nodesXmlInfoLabel.textScale = 0.6f;
            y += vertPadding/2 + 2;

            SetLabel(waysXmlInfoLabel, "waysXmlInfoLabel", x, y);
            waysXmlInfoLabel.textScale = 0.6f;
            y += vertPadding/2 + 2;

            SetLabel(roadMakerInfoLabel, "roadmakerInfoLabel", x, y);
            roadMakerInfoLabel.textScale = 0.6f;
            y += vertPadding/2 + 2;

            SetLabel(osmInfoLabel, "osmInfoLabel", x, y);
            osmInfoLabel.textScale = 0.6f;
            y += vertPadding/2 + 2;



            SetLabel(errorLabel, "No Error", x, y);
            errorLabel.textScale = 0.6f;
            y += vertPadding + 12;

            SetButton(makeRoadsButton, "Make Roads", y);
            makeRoadsButton.eventClick += makeRoadsButton_eventClick;
            makeRoadsButton.Disable();

            height = y + vertPadding;
        }

        // Called when TarrainParty button is pressed 
        //  - starts downloading async if request okay
        //  - calls client_DownloadDataCompleted on completion
        private void loadTerrainParty_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            try
            {
                decimal startLat = 0M;
                decimal startLon = 0M;
                decimal endLat = 0M;
                decimal endLon = 0M;
                var sc = double.Parse(scaleTextBox.text);
                if (!GetCoordinates(ref startLon, ref startLat, ref endLon, ref endLat))
                {
                    return;
                }
                var client = new WebClient();
                client.Headers.Add("user-agent", "Cities Skylines Mapping Mod v1");
                client.DownloadDataCompleted += client_DownloadDataCompleted;
                client.DownloadDataAsync(
                    new System.Uri(string.Format(
                        "http://terrain.party/api/export?box={0},{1},{2},{3}&heightmap=merged", endLon, endLat, startLon,
                        startLat)));
                errorLabel.text = "Downloading map from Terrain.Party...";
            }
            catch (Exception ex)
            {
                //errorLabel.text = ex.ToString();
            }
        }

        // LoadsHeightMap into C:S - called by client_DownloadDataCompleted
        private IEnumerator LoadHeightMap16(byte[] heightmap)
        {
            bool ok = false;
            try
            {
                infoLabel.text = "bytes:" + heightmap.Length;
                Singleton<TerrainManager>.instance.SetHeightMap16(heightmap);
                ok = true;
            }
            catch (Exception ex)
            {
                errorLabel.text = ex.ToString();
            }
            if (ok)
            {
                errorLabel.text = "Terrain Loaded";
            }
            yield return null;
        }

        // Checks status of hight mad and calls LoadHeightMap16
        private void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                var image = new Image(e.Result);
                //Debug.Log("1-image.w" + image.width + " h:" + image.height);
                image.Convert(Image.kFormatAlpha16);
                //Debug.Log("2-image.w" + image.width + " h:" + image.height);
                if (image.width != 1081 || image.height != 1081)
                {
                    if (!image.Resize(1081, 1081))
                    {
                        //Debug.Log("3-image.w" + image.width + " h:" + image.height);
                        errorLabel.text = string.Concat(new object[]
                        {
                            "Resize not supported: ",
                            image.format,
                            "-",
                            image.width,
                            "x",
                            image.height,
                            " Expected: ",
                            1081,
                            "x",
                            1081
                        });
                        return;
                    }
                    //Debug.Log("4-image.w" + image.width + " h:" + image.height);

                }
                m_LastHeightmap16 = image.GetPixels();
                //Debug.Log("5-image.w" + image.width + " h:" + image.height);
                //infoLabel.text = "w:" + image.width + " h:" + image.height;
                //m_LastHeightmap16 = image.GetPixels();
                //Debug.Log("6-lhm15bytes:" +m_LastHeightmap16.Length);
                Singleton<TerrainManager>.instance.SetHeightMap16(m_LastHeightmap16);
                //Debug.Log("7-done");
                //SimulationManager.instance.AddAction(LoadHeightMap16(m_LastHeightmap16));
            }
            catch (Exception ex)
            {
                //errorLabel.text = ex.ToString();
            }
        }

        // toggles highways Checkbox
        private void highwaysCheck_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            highways = !highways;
            highwaysCheck.text = highways.ToString();
        }

        // toggles roads Checkbox
        private void roadsCheck_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            roads = !roads;
            roadsCheck.text = roads.ToString();
        }

        // toggles pedestrians Checkbox
        private void pedestriansCheck_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            peds = !peds;
            pedestriansCheck.text = peds.ToString();
        }


public bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
{
    bool isOk = true;
    // If there are errors in the certificate chain,
    // look at each error to determine the cause.
    if (sslPolicyErrors != SslPolicyErrors.None) {
        for (int i=0; i<chain.ChainStatus.Length; i++) {
            if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) {
                continue;
            }
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            bool chainIsValid = chain.Build ((X509Certificate2)certificate);
            if (!chainIsValid) {
                isOk = false;
                break;
            }
        }
    }
    return isOk;
}
        private void loadAPIButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            try
            {
                ob = GetBounds();

                //var streetMapRequest = new OpenStreeMapFrRequest(ob);
                //var streetMapRequest = new OpenStreeMapApi0_6Request(ob);
                var streetMapRequest = new OverpassRequest(ob);

                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, streetMapRequest.NodeRequestUrl);
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, streetMapRequest.WaysRequestUrl);

                var nodesWebClient = new WebClient();
                nodesWebClient.DownloadDataCompleted += NodesWebClientCallback;
                Debug.Log("NodeRequestUrl:" + streetMapRequest.NodeRequestUrl);
                nodesWebClient.DownloadDataAsync(new Uri(streetMapRequest.NodeRequestUrl));

                var waysWebClient = new WebClient();
                Debug.Log("WaysRequestUrl:" + streetMapRequest.WaysRequestUrl);
                waysWebClient.DownloadDataCompleted += WaysWebClientCallback;
                waysWebClient.DownloadDataAsync(new Uri(streetMapRequest.WaysRequestUrl));
            }
            catch (Exception ex)
            {
                errorLabel.text = ex.ToString();
            }
        }

        private void NodesWebClientCallback(object sender, DownloadDataCompletedEventArgs e)
        {
            nodesXml = e.Result;
            Debug.Log("NodesWebClientCallback e.Error:" + e.Error);
            var msg = "Node Data Loaded";
            if (nodesXml!=null)
            {
                msg += " len:"+nodesXml.Length;
            }
            else
            {
                msg += " but nodesXml is null";
            }
            errorLabel.text = msg;
			nodesLoaded = true;
			if (nodesLoaded && waysLoaded) { 
				makeRoadsButton.Enable();
			}
        }

        private void WaysWebClientCallback(object sender, DownloadDataCompletedEventArgs e)
        {
            waysXml = e.Result;
            //errorLabel.text = string.Format("{0} Data Loaded.", "Ways");
			waysLoaded = true;
			if (nodesLoaded && waysLoaded)
			{
				makeRoadsButton.Enable();
			}
        }

        private bool GetCoordinates(ref decimal startLon, ref decimal startLat, ref decimal endLon, ref decimal endLat)
        {
            var text = coordinates.text.Trim();
            var split = text.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
            decimal midLon = 0M;
            decimal midLat = 0M;

            if (split.Count() == 2)
            {
                if (!decimal.TryParse(split[0], out midLon) || !decimal.TryParse(split[1], out midLat))
                {
                    errorLabel.text = "Coordinates must be numbers.";
                    return false;
                }
            }
            else if (split.Count() == 4)
            {
                if (!decimal.TryParse(split[0], out endLon) || !decimal.TryParse(split[1], out startLat) ||
                    !decimal.TryParse(split[2], out startLon) || !decimal.TryParse(split[3], out endLat))
                {
                    errorLabel.text = "Coordinates must be numbers.";
                    return false;
                }
                midLon = (endLon + startLon) / 2M;
                midLat = (endLat + startLat) / 2M;
            }
            else
            {
                errorLabel.text =
                    "Coordinate format wrong! Input either one or two sets of coordinates seperated by commas.";
                return false;
            }

            return true;
        }

        private OSM.osmBounds GetBounds()
        {
            decimal startLat = 0M;
            decimal startLon = 0M;
            decimal endLat = 0M;
            decimal endLon = 0M;

            if (!GetCoordinates(ref startLon, ref startLat, ref endLon, ref endLat))
            {
                return null;
            }

            var ob = new osmBounds();
            ob.minlon = startLon;
            ob.minlat = startLat;
            ob.maxlon = endLon;
            ob.maxlat = endLat;
            return ob;
        }

        // Called when Load OSM file button is clicked
        //  -   Disables loadMapButton and LoadAPIButton so they do not work anymore!!!!
        private void loadOsmFromFileButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            var ob = GetBounds();


            var path = pathTextBox.text.Trim();
            if (!File.Exists(path))
            {
                path += ".osm";
                if (!File.Exists(path))
                {
                    //errorLabel.text = "Cannot find osm file: " + path;
                    return;
                }
            }
            try
            {
                var osm = new OSMInterface(ob, pathTextBox.text.Trim(), double.Parse(scaleTextBox.text.Trim()),
                    double.Parse(tolerance.text.Trim()), double.Parse(curveTolerance.text.Trim()),
                    double.Parse(tiles.text.Trim()));
                currentIndex = 0;
                roadMaker = new RoadMaker2(osm);
                //errorLabel.text = "File Loaded.";
                makeRoadsButton.Enable();
                loadOsmFromFile.Disable();
                loadAPIButton.Disable();
            }
            catch (Exception ex)
            {
                //errorLabel.text = ex.ToString();
            }
        }

        private void SetButton(UIButton makeRoadsButton, string p1, int x, int y)
        {
            makeRoadsButton.text = p1;
            makeRoadsButton.normalBgSprite = "ButtonMenu";
            makeRoadsButton.hoveredBgSprite = "ButtonMenuHovered";
            makeRoadsButton.disabledBgSprite = "ButtonMenuDisabled";
            makeRoadsButton.focusedBgSprite = "ButtonMenuFocused";
            makeRoadsButton.pressedBgSprite = "ButtonMenuPressed";
            makeRoadsButton.size = new Vector2(50, 18);
            makeRoadsButton.relativePosition = new Vector3(x, y - 3);
            makeRoadsButton.textScale = 0.8f;
        }

        private void SetButton(UIButton button, string p1, int y)
        {
            button.text = p1;
            button.normalBgSprite = "ButtonMenu";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.focusedBgSprite = "ButtonMenuFocused";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.size = new Vector2(260, 24);
            button.relativePosition = new Vector3((int) (width - button.size.x) / 2, y);
            button.textScale = 0.8f;
        }

        private void SetCheckBox(UICustomCheckbox3 pedestriansCheck, int x, int y)
        {
            pedestriansCheck.IsChecked = true;
            pedestriansCheck.relativePosition = new Vector3(x, y);
            pedestriansCheck.size = new Vector2(13, 13);
            pedestriansCheck.Show();
            pedestriansCheck.color = new Color32(185, 221, 254, 255);
            pedestriansCheck.enabled = true;
            pedestriansCheck.spriteName = "AchievementCheckedFalse";
            pedestriansCheck.eventClick +=
                (component, param) => { pedestriansCheck.IsChecked = !pedestriansCheck.IsChecked; };
        }

        private void SetTextBox(UITextField scaleTextBox, string p, int x, int y)
        {
            scaleTextBox.relativePosition = new Vector3(x, y - 4);
            scaleTextBox.horizontalAlignment = UIHorizontalAlignment.Left;
            scaleTextBox.text = p;
            scaleTextBox.textScale = 0.8f;
            scaleTextBox.color = Color.black;
            scaleTextBox.cursorBlinkTime = 0.45f;
            scaleTextBox.cursorWidth = 1;
            scaleTextBox.selectionBackgroundColor = new Color(233, 201, 148, 255);
            scaleTextBox.selectionSprite = "EmptySprite";
            scaleTextBox.verticalAlignment = UIVerticalAlignment.Middle;
            scaleTextBox.padding = new RectOffset(5, 0, 5, 0);
            scaleTextBox.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            scaleTextBox.normalBgSprite = "TextFieldPanel";
            scaleTextBox.hoveredBgSprite = "TextFieldPanelHovered";
            scaleTextBox.focusedBgSprite = "TextFieldPanel";
            scaleTextBox.size = new Vector3(width - 120 - 30, 20);
            scaleTextBox.isInteractive = true;
            scaleTextBox.enabled = true;
            scaleTextBox.readOnly = false;
            scaleTextBox.builtinKeyNavigation = true;
        }

        private void SetLabel(UILabel pedestrianLabel, string p, int x, int y)
        {
            pedestrianLabel.relativePosition = new Vector3(x, y);
            pedestrianLabel.text = p;
            pedestrianLabel.textScale = 0.8f;
            pedestrianLabel.size = new Vector3(120, 20);
        }

        private void makeRoadsButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            //var scale = double.Parse(scaleTextBox.text.Trim());
            //var tt = double.Parse(tiles.text.Trim());
            //var osm = new OSMInterface(ob, nodesXml, waysXml, scale, double.Parse(tolerance.text.Trim()),double.Parse(curveTolerance.text.Trim()), tt);
            Debug.Log("MakeRoads Button eventClick");
            currentIndex = 0;
            //roadMaker = new RoadMaker2(osm);
            loadOsmFromFile.Disable();
            loadAPIButton.Disable();

            Debug.Log("Before if");
            if (roadMaker != null)
            {
                createRoads = !createRoads;
            }
            createRoads = true;
            var pp = peds;
            var rr = roads;
            var hh = highways;
            Debug.Log("Before Make1");
            roadMaker.Make(1, pp, rr, hh);
            roadMaker.Make(2, pp, rr, hh);
            roadMaker.Make(3, pp, rr, hh);
        }

        public override void Update()
        {
            makeRoadsButton.Enable();
            if (createRoads)
            {
                Debug.Log("Creating Roads");
                var pp = peds;
                var rr = roads;
                var hh = highways;
                if (currentIndex < roadMaker.osm.ways.Count())
                {
                    //Debug.Log("AddAction 1");
                    SimulationManager.instance.AddAction(roadMaker.Make(currentIndex, pp, rr, hh));
                    currentIndex += 1;
                }

                if (currentIndex < roadMaker.osm.ways.Count())
                {
                    //Debug.Log("AddAction 2");
                    SimulationManager.instance.AddAction(roadMaker.Make(currentIndex, pp, rr, hh));
                    currentIndex += 1;
                }

                if (currentIndex < roadMaker.osm.ways.Count())
                {
                    //Debug.Log("AddAction 3");
                    SimulationManager.instance.AddAction(roadMaker.Make(currentIndex, pp, rr, hh));
                    currentIndex += 1;
                    var instance = Singleton<NetManager>.instance;
                    infoLabel.text = String.Format("Making road {0} out of {1}. Nodes: {2}. Segments: {3}",
                        currentIndex, roadMaker.osm.ways.Count(), instance.m_nodeCount, instance.m_segmentCount);
                }
            }

            nodesXmlInfoLabel.text = "nodesXml:"+ (nodesXml==null?"null":nodesXml.Length.ToString());
            waysXmlInfoLabel.text = "waysXml:" + (waysXml == null ? "null" : waysXml.Length.ToString());
            lastHeightMap16InfoLabel.text = "m_lastHeightmap16:" + (m_LastHeightmap16 == null ? "null" : m_LastHeightmap16.Length.ToString());
            roadMakerInfoLabel.text = "roadMaker:" + (roadMaker == null ? "null" : roadMaker.ToString());
            if (roadMaker!=null)
            {
                var osm = roadMaker.osm;
                osmInfoLabel.text = "osmMaker:" + (osm == null ? "null" : " n:"+osm.nodes.Count.ToString()+ " w:"+osm.ways.Count.ToString());
            }

            if (roadMaker != null && currentIndex == roadMaker.osm.ways.Count())
            {
                errorLabel.text = "Done.";
                createRoads = false;
            }
            base.Update();
        }
    }

    public class UICustomCheckbox3 : UISprite
    {
        public bool IsChecked { get; set; }

        public override void Start()
        {
            base.Start();
            IsChecked = true;
            spriteName = "AchievementCheckedTrue";
        }

        public override void Update()
        {
            base.Update();
            spriteName = IsChecked ? "AchievementCheckedTrue" : "AchievementCheckedFalse";
        }
    }
}
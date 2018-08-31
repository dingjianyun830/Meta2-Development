﻿// Copyright © 2018, Meta Company.  All rights reserved.
// 
// Redistribution and use of this software (the "Software") in source and binary forms, with or 
// without modification, is permitted provided that the following conditions are met:
// 
// 1.      Redistributions in source code must retain the above copyright notice, this list of 
//         conditions and the following disclaimer.
// 2.      Redistributions in binary form must reproduce the above copyright notice, this list of 
//         conditions and the following disclaimer in the documentation and/or other materials 
//         provided with the distribution.
// 3.      The name of Meta Company (“Meta”) may not be used to endorse or promote products derived 
//         from this software without specific prior written permission from Meta.
// 4.      LIMITATION TO META PLATFORM: Use of the Software and of any and all libraries (or other 
//         software) incorporating the Software (in source or binary form) is limited to use on or 
//         in connection with Meta-branded devices or Meta-branded software development kits.  For 
//         example, a bona fide recipient of the Software may modify and incorporate the Software 
//         into an application limited to use on or in connection with a Meta-branded device, while 
//         he or she may not incorporate the Software into an application designed or offered for use 
//         on a non-Meta-branded device.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL META COMPANY BE LIABLE FOR ANY DIRECT, 
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using System.Runtime.InteropServices;
using UnityEngine;
using MetaCoreInterop = Meta.Interop.MetaCoreInterop;

using System.IO;
using System;
using Texture = UnityEngine.Texture2D;
/// <summary>
/// Structure to hold marshalled points
/// </summary>
public struct PointCloud
{
    public int num_points;
    public float[] points;
} 

/// <summary>
/// Example of how to use the meta_get_point_cloud() api 
/// This example will lead to a performance hit and is NOT recommended for use in production applications.
/// This is only an example of the intended usage of the API.
/// </summary>
public class MetaPointCloudExample : MonoBehaviour {

    MetaCoreInterop.MetaPointCloud _metaPointCloud = new MetaCoreInterop.MetaPointCloud();
    PointCloud _pointCloud = new PointCloud();

    /// <summary>
    /// Amount of memory to alloc for marshalling the entire point cloud
    /// TODO: provide API for image sizes and info
    /// </summary>
    private const int POINT_CLOUD_WIDTH = 352;
    private const int POINT_CLOUD_HEIGHT = 287;
    private const int VERTEX_STRIDE = 3;
    private const int POINT_CLOUD_SIZE = POINT_CLOUD_WIDTH * POINT_CLOUD_HEIGHT * VERTEX_STRIDE * sizeof(float);

    /// <summary>
    /// Mesh for rendering point cloud
    /// </summary>
    private Mesh _mesh;

    /// <summary>
    /// Max points for point cloud mesh
    /// NOTE: this will not render the entire point cloud marshalled above
    /// </summary>
    private const int MAX_POINTS = 61440;
    private Vector3[] _verts = new Vector3[MAX_POINTS];
    private int[] _indices = new int[MAX_POINTS];

    /// <summary>
    /// Current translation and rotation of the depth sensor w-r-t world
    /// </summary>
    double[] _translation = new double[3];
    double[] _rotation = new double[4];
    double[] _new_rotation = new double[4];

    /// <summary>
    /// Should this script render the point cloud 
    /// </summary>
    public bool RenderPointCloud = false;

    //////////////////////////////////Added by Yuqi Ding

    // The folder to contain our screenshots.
    // If the folder exists we will append numbers to create an empty folder.
    string folder = "D:\\shotFolder1";
    int frameRate = 24;

    public bool SavePointCloud = true;

    // Constants
    private const int TextureWidth = 1280;
    private const int TextureHeight = 720;
    private const TextureFormat TextureFormat = UnityEngine.TextureFormat.RGB24;
    private const int BitsPerPixel = 32;
    private const bool EnableMipmap = false;

    // Data
    private Texture _rgbTexture = null;
    private IntPtr RawPixelBuffer;
    private int _totalBufferSize = 0;

    // save the point cloud to .ply file
    private void SavePointCloudToPly(string filename, PointCloud pointCloud)
    {
        StreamWriter plyfile = new StreamWriter(filename);

        // write the header
        plyfile.WriteLine("ply");
        plyfile.WriteLine("format ascii 1.0");
        plyfile.WriteLine("comment author: Yuqi Ding");
        plyfile.WriteLine("element vertex {0}", pointCloud.num_points);
        plyfile.WriteLine("property float64 x");
        plyfile.WriteLine("property float64 y");
        plyfile.WriteLine("property float64 z");
        plyfile.WriteLine("end_header");
        for (int i = 0; i < pointCloud.num_points * 3; i = i + 3)
        {
            plyfile.WriteLine("{0} {1} {2}",_pointCloud.points[i],_pointCloud.points[i + 1], _pointCloud.points[i + 2]);
        }
        plyfile.Close();
    }
    //Added end

    // Use this for initialization
    void Start () {
        _mesh = GetComponent<MeshFilter>().mesh;
        _mesh.Clear();

        // Alloc memory for point cloud (only once)
        _metaPointCloud.points = Marshal.AllocHGlobal( POINT_CLOUD_SIZE );
        _pointCloud.points = new float[POINT_CLOUD_SIZE];

        _totalBufferSize = TextureWidth * TextureHeight * (BitsPerPixel / 8);
        RawPixelBuffer = Marshal.AllocHGlobal(_totalBufferSize);
        _rgbTexture = new Texture(TextureWidth, TextureHeight, TextureFormat, EnableMipmap);

        // Cpature by frame rate
        // Set the playback framerate
        Time.captureFramerate = frameRate;

        // Create the folder
        System.IO.Directory.CreateDirectory(folder);
    }

    // Update is called once per frame
    void Update()
    {
        MetaCoreInterop.meta_get_point_cloud(ref _metaPointCloud, _translation, _rotation);
        //Added by Yuqi Ding

        MetaCoreInterop.meta_get_rgb_frame(RawPixelBuffer, _translation, _new_rotation);  // The buffer is pre-allocated by constructor.

        // Check for a difference
        bool isEqual = true;

        // Check for a difference in pose (should change with each new RGB frame).
        for (int i = 0; i < _new_rotation.Length; ++i)
        {
            isEqual = _rotation[i] == _new_rotation[i];

            if (!isEqual) break;
        }

        // If the two rotations are not equal, we have a new rgb frame. 
        if (!isEqual)
        {
            // Copy new rotation if it's different.
            for (int i = 0; i < _new_rotation.Length; ++i)
            {
                _rotation[i] = _new_rotation[i];
            }

            _rgbTexture.LoadRawTextureData(RawPixelBuffer, _totalBufferSize);
            _rgbTexture.Apply();
            if(Time.frameCount%48==0)
            {
                byte[] bytes = _rgbTexture.EncodeToPNG();
                string rgbname = string.Format("{0}/{1:D04} shot.png", folder, Time.frameCount);
                File.WriteAllBytes(rgbname, bytes);
            }

        }
        // Added end

        SetDepthToWorldTransform();

        if (SavePointCloud&&(Time.frameCount%48==0))
        {
            MarshalMetaPointCloud();
            //UpdateMesh();
            int num = _metaPointCloud.num_points;
            Debug.Log(_translation[0].ToString());
            Debug.Log(_translation[1].ToString());
            Debug.Log(_translation[2].ToString());
            if(num!=0)
            {
                string Name1 = string.Format("{0}/{1:D04} shot.ply", folder, Time.frameCount);
                SavePointCloudToPly(Name1, _pointCloud);
            }

        }
    }

    private void UpdateMesh()
    {
        for (int i = 0; i < MAX_POINTS; ++i)
        {
            _verts[i].Set(_pointCloud.points[(i * VERTEX_STRIDE) + 0],
                         -_pointCloud.points[(i * VERTEX_STRIDE) + 1], // flip to unity handedness
                          _pointCloud.points[(i * VERTEX_STRIDE) + 2]);
            _indices[i] = i;
        }

        _mesh.Clear();
        _mesh.vertices = _verts;
        _mesh.SetIndices(_indices, MeshTopology.Points, 0);
    }

    private void SetDepthToWorldTransform()
    {
        transform.position = new Vector3((float)_translation[0],
                                         (float)_translation[1],
                                         (float)_translation[2]);

        transform.rotation = new Quaternion((float)_rotation[0],
                                            (float)_rotation[1],
                                            (float)_rotation[2],
                                            (float)_rotation[3]);
    }

    private void MarshalMetaPointCloud()
    {
        _pointCloud.num_points = _metaPointCloud.num_points;

        int point_cloud_size = 3 * _pointCloud.num_points;

        Marshal.Copy(_metaPointCloud.points,
                      _pointCloud.points,
                      0, point_cloud_size);
    }
}

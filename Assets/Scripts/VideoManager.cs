using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public int currentVideoIndex = 0;
    public List<string> videos = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        videos = GetVideos();
    }

    public void PlayVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.clip = Resources.Load<VideoClip>("Videos/" + videos[currentVideoIndex]);
        videoPlayer.Play();
    }

    public void NextVideo()
    {
        currentVideoIndex++;
        if (currentVideoIndex >= videos.Count)
        {
            currentVideoIndex = 0;
        }
        PlayVideo();
    }

    public void PreviousVideo()
    {
        currentVideoIndex--;
        if (currentVideoIndex < 0)
        {
            currentVideoIndex = videos.Count - 1;
        }
        PlayVideo();
    }

    //check if video is finished playing and play next video
    void Update()
    {
       
        //pressing the n key will play the next video
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextVideo();
        }
  
    }

    //function to return the list of videos(for video player from the folder Assets/Resources/Videos
    public static List<string> GetVideos()
    {
        List<string> videos = new List<string>();
        Object[] objects = Resources.LoadAll("Videos", typeof(VideoClip));
        foreach (Object obj in objects)
        {
            videos.Add(obj.name);
        }
        return videos;
    }
   
}

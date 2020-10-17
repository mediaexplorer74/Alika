﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Alika.Libs.VK.Responses
{
    public struct Attachment
    {
        [JsonProperty("type")]
        public string type;
        [JsonProperty("photo")]
        public Photo photo;
        [JsonProperty("video")]
        public Video video;
        [JsonProperty("audio")]
        public Audio audio;
        [JsonProperty("doc")]
        public Document document;
        [JsonProperty("graffiti")]
        public Graffiti graffiti;
        [JsonProperty("audio_message")]
        public AudioMessage audio_message;
        [JsonProperty("link")]
        public Link link;
        [JsonProperty("wall")]
        public Wall wall;
        [JsonProperty("wall_reply")]
        public WallReply wall_reply;
        [JsonProperty("sticker")]
        public Sticker sticker;
        [JsonProperty("gift")]
        public Gift gift;

        public class Photo : AttachBase
        {
            [JsonProperty("album_id")]
            public int album_id { get; set; }
            [JsonProperty("user_id")]
            public int user_id { get; set; }
            [JsonProperty("text")]
            public string text { get; set; }
            [JsonProperty("date")]
            public int dateUnix { get; set; }
            [JsonProperty("sizes")]
            public List<Size> sizes { get; set; }

            public Size GetBestQuality() => this.sizes.OrderByDescending(i => i.width).ThenByDescending(i => i.height).First();

            public override string ToAttachFormat() => "photo" + base.ToAttachFormat();

            public class Size
            {
                [JsonProperty("type")]
                public string type { get; set; }
                [JsonProperty("url")]
                public string url { get; set; }
                [JsonProperty("width")]
                public int width { get; set; }
                [JsonProperty("height")]
                public int height { get; set; }

            }
        }
        public class Video : AttachBase
        {
            [JsonProperty("title")]
            public string title { get; set; }
            [JsonProperty("description")]
            public string description { get; set; }
            [JsonProperty("duration")]
            public int duration { get; set; }
            [JsonProperty("photo_130")]
            public string photo_130 { get; set; }
            [JsonProperty("photo_320")]
            public string photo_320 { get; set; }
            [JsonProperty("photo_640")]
            public string photo_640 { get; set; }
            [JsonProperty("photo_800")]
            public string photo_800 { get; set; }
            [JsonProperty("photo_1280")]
            public string photo_1280 { get; set; }
            [JsonProperty("first_frame_130")]
            public string first_frame_130 { get; set; }
            [JsonProperty("first_frame_320")]
            public string first_frame_320 { get; set; }
            [JsonProperty("first_frame_640")]
            public string first_frame_640 { get; set; }
            [JsonProperty("first_frame_800")]
            public string first_frame_800 { get; set; }
            [JsonProperty("first_frame_1280")]
            public string first_frame_1280 { get; set; }
            [JsonProperty("date")]
            public int date { get; set; }
            [JsonProperty("adding_date")]
            public int adding_date { get; set; }
            [JsonProperty("views")]
            public int views { get; set; }
            [JsonProperty("comments")]
            public int comments { get; set; }
            [JsonProperty("player")]
            public string player { get; set; }
            [JsonProperty("platform")]
            public string platform { get; set; }
            [JsonProperty("can_edit")]
            public int can_edit { get; set; }
            [JsonProperty("can_add")]
            public int can_add { get; set; }
            [JsonProperty("is_private")]
            public int is_private { get; set; }
            [JsonProperty("processing")]
            public int processing { get; set; }
            [JsonProperty("live")]
            public int live { get; set; }
            [JsonProperty("upcoming")]
            public int upcoming { get; set; }
            [JsonProperty("is_favorite")]
            public bool is_favorite { get; set; }

            public override string ToAttachFormat() => "video" + base.ToAttachFormat();
        }
        public class Audio : AttachBase
        {
            [JsonProperty("artist")]
            public string artist { get; set; }
            [JsonProperty("title")]
            public string title { get; set; }
            [JsonProperty("duration")]
            public int duration { get; set; }
            [JsonProperty("url")]
            public string url { get; set; }
            [JsonProperty("lyrics_id")]
            public int lyrics_id { get; set; }
            [JsonProperty("album_id")]
            public int album_id { get; set; }
            [JsonProperty("genre_id")]
            public int genre_id { get; set; }
            [JsonProperty("date")]
            public int date { get; set; }
            [JsonProperty("no_search")]
            public int no_search { get; set; }
            [JsonProperty("is_hq")]
            public int is_hq { get; set; }

            public override string ToAttachFormat() => "audio" + base.ToAttachFormat();
        }
        public class Document : AttachBase
        {
            [JsonProperty("title")]
            public string title { get; set; }
            [JsonProperty("size")]
            public int size { get; set; }
            [JsonProperty("ext")]
            public string extension { get; set; }
            [JsonProperty("url")]
            public string url { get; set; }
            [JsonProperty("date")]
            public string date { get; set; }
            [JsonProperty("type")]
            public string type { get; set; }
            [JsonProperty("preview")]
            public Preview preview { get; set; }

            public override string ToAttachFormat() => "doc" + base.ToAttachFormat();

            public class Preview
            {
                [JsonProperty("photo")]
                public Photo photo { get; set; }
                [JsonProperty("graffiti")]
                public Graffiti graffiti { get; set; }
                [JsonProperty("audio_message ")]
                public AudioMessage audio_message { get; set; }
            }
        }
        public class Graffiti : AttachBase
        {
            [JsonProperty("url")]
            public string url { get; set; }
            [JsonProperty("width")]
            public int width { get; set; }
            [JsonProperty("height")]
            public int height { get; set; }

            public override string ToAttachFormat() => "doc" + base.ToAttachFormat();
        }
        public class AudioMessage : AttachBase
        {
            [JsonProperty("duration")]
            public int duration { get; set; }
            [JsonProperty("waveform")]
            public List<int> waveform { get; set; }
            [JsonProperty("link_ogg")]
            public string link_ogg { get; set; }
            [JsonProperty("link_mp3")]
            public string link_mp3 { get; set; }
            [JsonProperty("transcript")]
            public string transcript { get; set; }
            [JsonProperty("transcript_state")]
            public string transcript_state { get; set; }

            public override string ToAttachFormat() => "doc" + base.ToAttachFormat();
        }
        public class Link
        {
            [JsonProperty("url")]
            public string url { get; set; }
            [JsonProperty("title")]
            public string title { get; set; }
            [JsonProperty("caption ")]
            public string caption { get; set; }
            [JsonProperty("description")]
            public string description { get; set; }
            [JsonProperty("photo")]
            public Photo photo { get; set; }

        }
        public class Wall
        {
            [JsonProperty("id")]
            public int id { get; set; }
            [JsonProperty("owner_id")]
            public int owner_id { get; set; }
            [JsonProperty("to_id")]
            public int to_id { get; set; }
            [JsonProperty("from_id")]
            public int from_id { get; set; }
            [JsonProperty("date")]
            public int date { get; set; }
            [JsonProperty("text")]
            public string text { get; set; }
            [JsonProperty("reply_owner_id")]
            public int reply_owner_id { get; set; }
            [JsonProperty("reply_post_id")]
            public int reply_post_id { get; set; }
            [JsonProperty("friends_only")]
            public int friends_only { get; set; }
        }
        public class WallReply
        {
            [JsonProperty("id")]
            public int id { get; set; }
            [JsonProperty("from_id")]
            public int from_id { get; set; }
            [JsonProperty("post_id")]
            public int post_id { get; set; }
            [JsonProperty("owner_id")]
            public int owner_id { get; set; }
            [JsonProperty("date")]
            public int date { get; set; }
            [JsonProperty("text")]
            public string text { get; set; }
            [JsonProperty("reply_to_user")]
            public int reply_to_user { get; set; }
            [JsonProperty("reply_to_comment")]
            public int reply_to_comment { get; set; }
        }
        public class Sticker
        {
            [JsonProperty("product_id")]
            public int product_id { get; set; }
            [JsonProperty("sticker_id")]
            public int sticker_id { get; set; }
            [JsonProperty("images")]
            public List<Image> images { get; set; }
            [JsonProperty("images_with_background")]
            public List<Image> images_with_background { get; set; }
            [JsonProperty("animation_url")]
            public string animation_url { get; set; }
            [JsonProperty("is_allowed")]
            public bool is_allowed { get; set; }

            public string GetBestQuality(bool background = false)
            {
                List<Image> sizes = background ? this.images_with_background : this.images;
                int width = 0;
                int height = 0;
                string url = "";
                sizes.ForEach((Image s) =>
                {
                    if (s.width >= width && s.height >= height) url = s.url;
                });
                return url;

            }

            public class Image
            {
                [JsonProperty("url")]
                public string url { get; set; }
                [JsonProperty("width")]
                public int width { get; set; }
                [JsonProperty("height")]
                public int height { get; set; }
            }
        }
        public class Gift
        {
            [JsonProperty("id")]
            public int id { get; set; }
            [JsonProperty("thumb_256")]
            public string thumb_256 { get; set; }
            [JsonProperty("thumb_96")]
            public string thumb_96 { get; set; }
            [JsonProperty("thumb_48")]
            public string thumb_48 { get; set; }
        }

        public abstract class AttachBase
        {
            [JsonProperty("id")]
            public int id { get; set; }
            [JsonProperty("owner_id")]
            public int owner_id { get; set; }
            [JsonProperty("access_key")]
            public string access_key { get; set; }

            public virtual string ToAttachFormat() => owner_id.ToString() + "_" + id.ToString() + (access_key?.Length > 0 ? "_" + access_key : "");
        }
    }

    public class GetHistoryAttachmentsResponse
    {
        [JsonProperty("items")]
        public List<AttachmentElement> items { get; set; }
        [JsonProperty("next_from")]
        public string next_from { get; set; }
        [JsonProperty("profiles")]
        public List<User> profiles { get; set; }
        [JsonProperty("groups")]
        public List<Group> groups { get; set; }

        public class AttachmentElement
        {
            [JsonProperty("message_id")]
            public int message_id { get; set; }
            [JsonProperty("from_id")]
            public int from_id { get; set; }
            [JsonProperty("attachment")]
            public Attachment attachment { get; set; }
        }
    }
}

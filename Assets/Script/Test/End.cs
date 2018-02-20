using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExAssetBundle;
using Zara.Common.ExBase;

namespace Zara.Test
{
    public class End : ExMonoBehaviour
    {
        [SerializeField]
        RawImage rawImage = null;

        [SerializeField]
        Image[] images = null;

        public void Init(Asset asset)
        {
            asset.Resource.Load("Image/End.png", (Texture texture) =>
            {
                rawImage.texture = texture;
            });

            asset.Resource.LoadMultipleAssets("Image/Lines.png", (Sprite[] sprites) =>
            {
                foreach (var pair in images.Zip(sprites, (i, s) => new { image = i, sprite = s }))
                    pair.image.sprite = pair.sprite;
            });
        }
    }
}
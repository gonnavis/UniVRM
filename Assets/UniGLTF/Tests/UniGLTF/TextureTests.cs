using NUnit.Framework;
using UnityEngine;
using VRMShaders;

namespace UniGLTF
{
    public class TextureTests
    {
        [Test]
        public void TextureExportTest()
        {
            // Dummy texture
            var tex0 = new Texture2D(128, 128)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Trilinear,
            };
            var textureManager = new TextureExporter();

            var material = new Material(Shader.Find("Standard"));
            material.mainTexture = tex0;

            var materialExporter = new MaterialExporter();
            materialExporter.ExportMaterial(material, textureManager);

            var convTex0 = textureManager.Exported[0];
            var sampler = TextureSamplerUtil.Export(convTex0);

            Assert.AreEqual(glWrap.CLAMP_TO_EDGE, sampler.wrapS);
            Assert.AreEqual(glWrap.CLAMP_TO_EDGE, sampler.wrapT);
            Assert.AreEqual(glFilter.LINEAR_MIPMAP_LINEAR, sampler.minFilter);
            Assert.AreEqual(glFilter.LINEAR_MIPMAP_LINEAR, sampler.magFilter);
        }


        [Test]
        public void NotReadable()
        {
            var readonlyTexture = Resources.Load<Texture2D>("4x4");
            Assert.False(readonlyTexture.isReadable);
            var (bytes, mime) = GltfTextureExporter.GetBytesWithMime(readonlyTexture);
            Assert.NotNull(bytes);
        }

        [Test]
        public void Compressed()
        {
            var readonlyTexture = Resources.Load<Texture2D>("4x4compressed");
            Assert.False(readonlyTexture.isReadable);
            var (bytes, mime) = GltfTextureExporter.GetBytesWithMime(readonlyTexture);
            Assert.NotNull(bytes);
        }

        [Test]
        public void ExportMetallicSmoothnessOcclusion_Test()
        {
            var metallic = new Texture2D(4, 4, TextureFormat.ARGB32, false, true);
            var occlusion = new Texture2D(4, 4, TextureFormat.ARGB32, false, true);

            {
                var exporter = new TextureExporter();
                Assert.AreEqual(-1, exporter.ExportMetallicSmoothnessOcclusion(null, 0, null));
            }
            {
                var exporter = new TextureExporter();
                Assert.AreEqual(0, exporter.ExportMetallicSmoothnessOcclusion(null, 0, occlusion));
                Assert.AreEqual(1, exporter.ExportMetallicSmoothnessOcclusion(metallic, 0, null));
            }
            {
                var exporter = new TextureExporter();
                Assert.AreEqual(0, exporter.ExportMetallicSmoothnessOcclusion(metallic, 0, occlusion));
                Assert.AreEqual(0, exporter.ExportMetallicSmoothnessOcclusion(null, 0, occlusion));
                Assert.AreEqual(0, exporter.ExportMetallicSmoothnessOcclusion(metallic, 0, null));
            }
        }
    }
}

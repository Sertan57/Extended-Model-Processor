using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace ExtendedModelProcessor
{
    [ContentProcessor(DisplayName = "ExtendedModelProcessor.ContentProcessor1")]
    public class ContentProcessor1 : ModelProcessor
    {
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            else
            {
                BoundingBox boundingBox;

                // Create variables to keep min and max xyz values for the model
                Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                ModelContent modelContent = base.Process(input, context);

                foreach (ModelMeshContent mesh in modelContent.Meshes)
                {
                    //Create variables to hold min and max xyz values for the mesh
                    Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                    Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                    // There may be multiple parts in a mesh (different materials etc.) so loop through each
                    foreach (ModelMeshPartContent part in mesh.MeshParts)
                    {
                        // The stride is how big, in bytes, one vertex is in the vertex buffer
                        int stride = (int)part.VertexBuffer.VertexDeclaration.VertexStride;

                        byte[] vertexData = part.VertexBuffer.VertexData;

                        // Find minimum and maximum xyz values for this mesh part
                        // We know the position will always be the first 3 float values of the vertex data
                        Vector3 vertPosition = new Vector3();
                        for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
                        {
                            vertPosition.X = BitConverter.ToSingle(vertexData, ndx);
                            vertPosition.Y = BitConverter.ToSingle(vertexData, ndx + sizeof(float));
                            vertPosition.Z = BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2);

                            // update our running values from this vertex
                            meshMin = Vector3.Min(meshMin, vertPosition);
                            meshMax = Vector3.Max(meshMax, vertPosition);
                        }
                    }

                    // Expand model extents by the ones from this mesh
                    modelMin = Vector3.Min(modelMin, meshMin);
                    modelMax = Vector3.Max(modelMax, meshMax);
                }
                boundingBox = new BoundingBox(modelMin, modelMax);
                modelContent.Tag = boundingBox;

                return modelContent;
            }
        }
    }
}
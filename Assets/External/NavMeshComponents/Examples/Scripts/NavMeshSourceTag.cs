using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
// @jsroo: modified to explicitly indicate the new navigation layer and shape
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    // Global containers for all active mesh/terrain tags
    public static List<NavMeshSourceTag> m_tags = new List<NavMeshSourceTag>();
    MeshFilter m_mesh;
    Collider m_collider;
    Terrain m_terrain;

    public enum SourceType
    {
        Collider, Mesh, Terrain
    }
    public SourceType source = SourceType.Mesh; // NOTE: works best with Meshes
    public NavMeshBuildSourceShape shape;
    public int area = 0; // NavMesh area index

    void OnEnable()
    {
        m_tags.Add(this);
        shape = NavMeshBuildSourceShape.Mesh;

        if (source == SourceType.Collider)
        {
            m_collider = GetComponent<Collider>();
            if (m_collider is BoxCollider)
            {
                shape = NavMeshBuildSourceShape.Box;
            }
            else if (m_collider is CapsuleCollider)
            {
                shape = NavMeshBuildSourceShape.Capsule;
            }
            else if (m_collider is SphereCollider)
            {
                shape = NavMeshBuildSourceShape.Sphere;
            }
            else
            {
                shape = NavMeshBuildSourceShape.Mesh;
            }
        }
        if (source == SourceType.Mesh)
        {
            m_mesh = GetComponent<MeshFilter>();
            shape = NavMeshBuildSourceShape.Mesh;
        }
        if (source == SourceType.Terrain)
        {
            m_terrain = GetComponent<Terrain>();
            shape = NavMeshBuildSourceShape.Terrain;
        }
    }

    void OnDisable()
    {
        m_tags.Remove(this);
    }

    // Collect all the navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();

        for (var i = 0; i < m_tags.Count; ++i)
        {
            if (m_tags[i].source == SourceType.Collider && m_tags[i].shape != NavMeshBuildSourceShape.Mesh)
            {
                var s = new NavMeshBuildSource();
                s.shape = m_tags[i].shape;
                s.transform = m_tags[i].m_collider.transform.localToWorldMatrix;
                s.area = m_tags[i].area;
                sources.Add(s);
            }
            else if (m_tags[i].source == SourceType.Mesh)
            {
                var mf = m_tags[i].m_mesh;
                if (mf == null) continue;

                var m = mf.sharedMesh;
                if (m == null) continue;

                var s = new NavMeshBuildSource();
                s.shape = NavMeshBuildSourceShape.Mesh;
                s.sourceObject = m;
                s.transform = mf.transform.localToWorldMatrix;
                s.area = m_tags[i].area;
                sources.Add(s);
            }
            else if (m_tags[i].source == SourceType.Terrain)
            {
                var t = m_tags[i].m_terrain;
                if (t == null) continue;

                var s = new NavMeshBuildSource();
                s.shape = NavMeshBuildSourceShape.Terrain;
                s.sourceObject = t.terrainData;
                // Terrain system only supports translation - so we pass translation only to back-end
                s.transform = Matrix4x4.TRS(t.transform.position, Quaternion.identity, Vector3.one);
                s.area = m_tags[i].area;
                sources.Add(s);
            }

        }
    }
}

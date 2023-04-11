using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Effect.PostProcessing
{
    public class PostProcessEffect : Effect
    {
        [SerializeField] PostProcessProfile _profile; public PostProcessProfile profile => _profile;
    }
}

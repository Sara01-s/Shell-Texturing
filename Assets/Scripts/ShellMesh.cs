using UnityEngine;

internal sealed class ShellMesh : MonoBehaviour {

	[SerializeField] private Mesh _mesh;
	[SerializeField] private Shader _shellShader;

	[SerializeField] private bool _updateOnValidate = false;

	[Range(1, 256)]
	[SerializeField] private uint _numShells = 256;
	[Range(1, 256)]
	[SerializeField] private int _numCells = 1;
	[Range(0.0f, 5.0f)]
	[SerializeField] private float _shellsSeparation = 1.0f;
	[Range(0.0f, 10.0f)]
	[SerializeField] private float _heightAttenuation = 1.0f;
	[Range(1.0f, 10.0f)]
	[SerializeField] private float _shellLength = 1.0f;
	[Range(0.0f, 100.0f)]
	[SerializeField] private float _cellThickness = 0.5f;

	[Header("Lighting")]
	[SerializeField] private Color _color;
	[SerializeField] private Color _shadowColor;
	[SerializeField] private Color _secondaryShadowColor;
	[Range(0.0f, 10.0f)]
	[SerializeField] private float _lightAttenuation = 1.0f;
	[Range(0.0f, 10.0f)]
	[SerializeField] private float _lightSmooth = 1.0f;
	[Range(0.0f, 1.0f)]
	[SerializeField] private float _shadowIntensity = 1.0f;

	private Material _material;
	private GameObject[] _layers;

	private void OnEnable() {
		GenerateShellMesh();
	}

	private void OnValidate() {
		if (_updateOnValidate && Application.isPlaying && _layers != null) {
			UpdateShellProperties();
		}
	}

	private void GenerateShellMesh() {
		_material = new Material(_shellShader);
		_layers = new GameObject[_numShells];

		for (int i = 0; i < _layers.Length; i++) {
			_layers[i] = new GameObject($"Shell {i}");
			_layers[i].transform.SetParent(transform, worldPositionStays: false);
			_layers[i].transform.localScale = Vector3.one * 2.0f;
			_layers[i].transform.rotation = Quaternion.AngleAxis(180.0f, Vector3.up);

			var shellMesh = _layers[i].AddComponent<MeshFilter>();
			var shellRenderer = _layers[i].AddComponent<MeshRenderer>();

			shellMesh.mesh = _mesh;
			shellRenderer.material = _material;
			shellRenderer.receiveShadows = false;
			shellRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}

		UpdateShellProperties();
	}

	private void UpdateShellProperties() {
		for (int i = 0; i < _layers.Length; i++) {
			var shellProperties = new MaterialPropertyBlock();

			shellProperties.SetInt("_NumCells", _numCells);
			shellProperties.SetInt("_NumShells", _layers.Length);
			shellProperties.SetInt("_ShellIndex", i);
			shellProperties.SetFloat("_ShellsSeparation", _shellsSeparation);
			shellProperties.SetFloat("_HeightAttenuation", _heightAttenuation);
			shellProperties.SetFloat("_ShellLength", _shellLength);
			shellProperties.SetFloat("_CellThickness", _cellThickness);

			shellProperties.SetColor("_Color", _color);
			shellProperties.SetColor("_ShadowColor", _shadowColor);
			shellProperties.SetColor("_SecondaryShadowColor", _secondaryShadowColor);
			shellProperties.SetFloat("_ShadowIntensity", 1.0f - _shadowIntensity);
			shellProperties.SetFloat("_LightAttenuation", _lightAttenuation);
			shellProperties.SetFloat("_LightSmooth", _lightSmooth);

			_layers[i].GetComponent<MeshRenderer>().SetPropertyBlock(shellProperties);
		}
	}

	private void OnDisable() {
		for (int i = 0; i < _layers.Length; i++) {
			Destroy(_layers[i]);
		}

		_layers = null;
	}

}
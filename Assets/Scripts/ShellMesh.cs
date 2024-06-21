using System.Collections.Generic;
using UnityEngine;

internal sealed class ShellMesh : MonoBehaviour {

	[SerializeField] private Mesh _mesh;
	[SerializeField] private Shader _shellShader;

	[SerializeField] private bool _updateOnValidate;

	[Range(1, 100)]
	[SerializeField] private uint _numShells;

	[Range(0.0f, 1000.0f)]
	[SerializeField] private float _density = 100.0f;

	private Material _material;
	private GameObject[] _layers;

	private void OnEnable() {
		GenerateShellMesh();
	}

	private void OnValidate() {
		if (_updateOnValidate) {
			UpdateShellProperties();
		}
	}

	private void GenerateShellMesh() {
		_material = new Material(_shellShader);
		_layers = new GameObject[_numShells];

		for (int i = 0; i < _layers.Length; i++) {
			_layers[i] = new GameObject($"Shell {i}");
			_layers[i].transform.SetParent(transform, worldPositionStays: false);
			_layers[i].transform.localScale = Vector3.one * 1000.0f;
			_layers[i].transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.left);

			var shellMesh = _layers[i].AddComponent<MeshFilter>();
			var shellRenderer = _layers[i].AddComponent<MeshRenderer>();

			shellMesh.mesh = _mesh;
			shellRenderer.SetMaterials(new List<Material>() { _material });
			shellRenderer.receiveShadows = false;
			shellRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			UpdateShellProperties();
		}
	}

	private void UpdateShellProperties() {
		for (int i = 0; i < _layers.Length; i++) {
			var shellProperties = new MaterialPropertyBlock();

			shellProperties.SetFloat("_Density", _density);
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
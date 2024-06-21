using System.Runtime.InteropServices;
using Random = UnityEngine.Random;
using UnityEngine;
using System;

internal sealed class ComputeExecutor : MonoBehaviour {

	struct Point {
		public Vector3 Position;
		public Matrix4x4 Matrix;
	}

	[SerializeField] private ComputeShader _computeShader;

	private void Awake() {
		RunComputeShader();
	}

	private void RunComputeShader() {

		Span<Point> points = stackalloc Point[5];
		Span<Point> pointsOutput = stackalloc Point[5];

		for (int i = 0; i < points.Length; i++) {
			points[i].Position = Random.insideUnitCircle;
			points[i].Matrix = Matrix4x4.identity;
		}
		
		var buffer = new ComputeBuffer(points.Length, Marshal.SizeOf<Point>() * points.Length);

		buffer.SetData(points.ToArray());

		int kernelIndex = _computeShader.FindKernel("Multiply");

		var renderTexture = new RenderTexture(width: 256, height: 256, depth: 24);
		renderTexture.enableRandomWrite = true;
		renderTexture.Create();

		_computeShader.SetBuffer(kernelIndex, "points", buffer);

		var threads = new Vector3Int(256 / 8, 256 / 8, 1);
		_computeShader.Dispatch(kernelIndex, threads.x, threads.y, threads.z);

		buffer.GetData(pointsOutput.ToArray());
		buffer.Dispose();

		for (int i = 0; i < pointsOutput.Length; i++) {
			print(pointsOutput[i].Position);
			print(pointsOutput[i].Matrix);
		}	
	}

}
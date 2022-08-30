package io.nyris.xamarincustomui

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import io.nyris.camera.CameraView

class MainActivity : AppCompatActivity() {
    lateinit var camera: CameraView
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        camera = findViewById(R.id.camera)
    }

    override fun onResume() {
        super.onResume()
        camera.start()
    }

    override fun onStop() {
        super.onStop()
        camera.start()
    }
}

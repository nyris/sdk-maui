package io.nyris.camera

import androidx.annotation.IntDef
import androidx.exifinterface.media.ExifInterface

/**
 *
 *
 * @author Sidali Mellouk
 * Created by nyris GmbH
 * Copyright © 2018 nyris GmbH. All rights reserved.
 */

@IntDef(ExifInterface.ORIENTATION_NORMAL, ExifInterface.ORIENTATION_FLIP_HORIZONTAL)
@kotlin.annotation.Retention(AnnotationRetention.SOURCE)
annotation class ExifOrientation
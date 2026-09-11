#!/usr/bin/env python3
"""
科学数据格式测试文件生成器
支持: netCDF-3, netCDF-4, HDF4, HDF5, GRIB1, GRIB2, BUFR
用于测试其他语言实现的读取兼容性
"""

import os
import sys
import numpy as np
from datetime import datetime, timedelta
import xarray as xr
import h5py
import netCDF4 as nc4

# 中文Windows控制台默认GBK编码，会因✓/✗字符抛UnicodeEncodeError导致脚本中断。
# 统一把stdout/stderr重设为UTF-8，保证在任何区域设置下都能完整跑完。
try:
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")
except Exception:
    pass

# GRIB和BUFR需要额外安装
try:
    import eccodes

    GRIB_AVAILABLE = True
except ImportError:
    GRIB_AVAILABLE = False
    print("警告: eccodes未安装，跳过GRIB和BUFR文件生成")

try:
    import h5py

    HDF5_AVAILABLE = True
except ImportError:
    HDF5_AVAILABLE = False
    print("警告: h5py未安装，跳过HDF5文件生成")

# HDF4需要pyhdf
try:
    from pyhdf.SD import SD, SDC

    HDF4_AVAILABLE = True
except ImportError:
    HDF4_AVAILABLE = False
    print("警告: pyhdf未安装，跳过HDF4文件生成")


def create_netcdf3_test_file(filepath):
    """创建netCDF-3格式测试文件"""
    print(f"\n创建netCDF-3测试文件: {filepath}")

    with nc4.Dataset(filepath, "w", format="NETCDF3_CLASSIC") as rootgrp:
        rootgrp.title = "NetCDF-3 Test File"
        rootgrp.institution = "Test Organization"
        rootgrp.source = "Python netCDF4"
        rootgrp.history = f"Created {datetime.now().isoformat()}"
        rootgrp.Conventions = "CF-1.8"

        time_dim = rootgrp.createDimension("time", 10)
        lat_dim = rootgrp.createDimension("lat", 5)
        lon_dim = rootgrp.createDimension("lon", 8)

        time = rootgrp.createVariable("time", "f8", ("time",))
        time.units = "hours since 2024-01-01 00:00:00"
        time.standard_name = "time"
        time[:] = np.arange(10) * 6

        lat = rootgrp.createVariable("lat", "f4", ("lat",))
        lat.units = "degrees_north"
        lat.standard_name = "latitude"
        lat[:] = np.linspace(-90, 90, 5)

        lon = rootgrp.createVariable("lon", "f4", ("lon",))
        lon.units = "degrees_east"
        lon.standard_name = "longitude"
        lon[:] = np.linspace(0, 360, 8, endpoint=False)

        temp = rootgrp.createVariable(
            "temperature", "f4", ("time", "lat", "lon"), fill_value=-999.0
        )
        temp.units = "K"
        temp.standard_name = "air_temperature"
        temp.long_name = "Air Temperature"
        temp.scale_factor = 0.1
        temp.add_offset = 273.15
        temp.coordinates = "lat lon"
        temp_data = 273.15 + 20 * np.random.randn(10, 5, 8)
        temp[:] = temp_data

        pressure = rootgrp.createVariable(
            "pressure", "i4", ("time", "lat", "lon"), fill_value=-9999
        )
        pressure.units = "hPa"
        pressure.standard_name = "air_pressure"
        pressure.long_name = "Air Pressure"
        pressure_data = 1000 + 50 * np.random.randn(10, 5, 8)
        pressure[:] = pressure_data.astype(np.int32)

        humidity = rootgrp.createVariable(
            "humidity", "i2", ("time", "lat", "lon"), fill_value=-32768
        )
        humidity.units = "percent"
        humidity.standard_name = "relative_humidity"
        humidity.long_name = "Relative Humidity"
        humidity.scale_factor = 0.01
        humidity.add_offset = 50.0
        humidity_data = 50 + 30 * np.random.randn(10, 5, 8)
        humidity[:] = (humidity_data / 0.01).astype(np.int16)

        flags = rootgrp.createVariable("quality_flags", "b", ("time", "lat", "lon"))
        flags.units = "1"
        flags.standard_name = "quality_flag"
        flags.long_name = "Quality Flags"
        flags.flag_values = "0,1,2,3"
        flags.flag_meanings = "good suspect bad missing"
        flags_data = np.random.randint(0, 4, size=(10, 5, 8))
        flags[:] = flags_data.astype(np.int8)

        rootgrp.createDimension("station_name_len", 20)
        station_names = rootgrp.createVariable(
            "station_names", "S1", ("lat", "station_name_len")
        )
        station_names.long_name = "Station Names"
        names = [f"STATION_{i:02d}" for i in range(5)]
        for i, name in enumerate(names):
            station_names[i, : len(name)] = np.frombuffer(name.encode("ascii"), dtype="S1")

    print(f"✓ netCDF-3文件创建完成: {filepath}")


def create_netcdf4_test_file(filepath):
    """创建netCDF-4格式测试文件 (包括压缩、分组、特性等)"""
    print(f"\n创建netCDF-4测试文件: {filepath}")

    with nc4.Dataset(filepath, "w", format="NETCDF4") as rootgrp:
        rootgrp.title = "NetCDF-4 Test File with Advanced Features"
        rootgrp.institution = "Test Organization"
        rootgrp.source = "Python netCDF4"
        rootgrp.history = f"Created {datetime.now().isoformat()}"
        rootgrp.Conventions = "CF-1.8"
        rootgrp.NCO = "netCDF-4 features: compression, groups, chunking"

        grp_satellite = rootgrp.createGroup("satellite_data")
        grp_ground = rootgrp.createGroup("ground_observations")

        # 地面数据组
        grp_ground.createDimension("time", 20)
        grp_ground.createDimension("station", 15)
        grp_ground.createDimension("level", 10)

        time = grp_ground.createVariable("time", "f8", ("time",))
        time.units = "seconds since 2024-01-01 00:00:00"
        time.standard_name = "time"
        time.calendar = "gregorian"
        time[:] = np.cumsum(np.random.randint(60, 3600, 20))

        station_id = grp_ground.createVariable("station_id", "i4", ("station",))
        station_id.long_name = "Station Identifier"
        station_id[:] = np.arange(100, 115)

        level = grp_ground.createVariable("level", "f4", ("level",))
        level.units = "m"
        level.standard_name = "height"
        level.positive = "up"
        level[:] = np.array([0, 10, 50, 100, 200, 500, 1000, 2000, 5000, 10000])

        temperature = grp_ground.createVariable(
            "temperature",
            "f4",
            ("time", "station", "level"),
            compression="zlib",
            complevel=5,
            chunksizes=(5, 5, 5),
            fill_value=-999.0,
        )
        temperature.units = "degC"
        temperature.standard_name = "air_temperature"
        temperature.long_name = "Air Temperature"
        temperature.scale_factor = 0.01
        temperature.add_offset = 20.0
        temp_data = 20 + 15 * np.random.randn(20, 15, 10)
        temperature[:] = temp_data

        pressure = grp_ground.createVariable(
            "pressure",
            "f4",
            ("time", "station", "level"),
            compression="zlib",
            complevel=5,
            chunksizes=(5, 5, 5),
            fill_value=-999.0,
        )
        pressure.units = "hPa"
        pressure.standard_name = "air_pressure"
        pressure.long_name = "Air Pressure"
        pressure_data = 1000 + 100 * np.random.randn(20, 15, 10)
        mask = np.random.random((20, 15, 10)) < 0.05
        pressure_data[mask] = -999.0
        pressure[:] = pressure_data

        # 卫星数据组
        grp_satellite.createDimension("scanline", 100)
        grp_satellite.createDimension("pixel", 80)
        grp_satellite.createDimension("band", 5)

        scanline = grp_satellite.createVariable("scanline", "i4", ("scanline",))
        scanline.units = "1"
        scanline.long_name = "Scan Line Number"
        scanline[:] = np.arange(100)

        pixel = grp_satellite.createVariable("pixel", "i4", ("pixel",))
        pixel.units = "1"
        pixel.long_name = "Pixel Number"
        pixel[:] = np.arange(80)

        band = grp_satellite.createVariable("band", "i4", ("band",))
        band.units = "1"
        band.long_name = "Spectral Band"
        band[:] = [1, 2, 3, 4, 5]

        radiance = grp_satellite.createVariable(
            "radiance",
            "f4",
            ("scanline", "pixel", "band"),
            compression="zlib",
            complevel=9,
            chunksizes=(20, 20, 2),
            shuffle=True,
            fletcher32=True,
        )
        radiance.units = "W/m^2/sr/μm"
        radiance.standard_name = "toa_radiance"
        radiance.long_name = "Top of Atmosphere Radiance"
        radiance.scale_factor = 0.001
        radiance.add_offset = 0.0
        radiance_data = 50 + 30 * np.random.randn(100, 80, 5)
        radiance[:] = np.maximum(0, radiance_data)

        cloud_mask = grp_satellite.createVariable(
            "cloud_mask",
            "b",
            ("scanline", "pixel"),
            compression="zlib",
            complevel=5,
            chunksizes=(20, 20),
        )
        cloud_mask.units = "1"
        cloud_mask.standard_name = "cloud_mask"
        cloud_mask.long_name = "Cloud Mask"
        cloud_mask.flag_values = "0,1,2,3"
        cloud_mask.flag_meanings = "clear_water clear_land cloud shadow"
        cloud_mask_data = np.random.randint(0, 4, size=(100, 80))
        cloud_mask[:] = cloud_mask_data.astype(np.int8)

        grp_satellite.createDimension("instrument_name_len", 30)
        instrument = grp_satellite.createVariable(
            "instrument_name", "S1", ("instrument_name_len",)
        )
        instrument.long_name = "Instrument Name"
        inst_name = "Test Radiometer v2.0"
        instrument[: len(inst_name)] = np.frombuffer(inst_name.encode("ascii"), dtype="S1")

        grp_satellite.createDimension("nvertices", 4)
        lat_bounds = grp_satellite.createVariable(
            "lat_bounds", "f4", ("scanline", "pixel", "nvertices")
        )
        lat_bounds.long_name = "Latitude Bounds"
        lat_bounds.units = "degrees_north"
        lat_bounds_data = np.random.uniform(-90, 90, (100, 80, 4))
        lat_bounds[:] = lat_bounds_data

        grp_compression = rootgrp.createGroup("compression_tests")
        grp_compression.createDimension("x", 1000)
        grp_compression.createDimension("y", 1000)

        for level in [1, 5, 9]:
            var = grp_compression.createVariable(
                f"compressed_level_{level}",
                "f4",
                ("x", "y"),
                compression="zlib",
                complevel=level,
                chunksizes=(100, 100),
            )
            var.long_name = f"Compression Level {level}"
            var[:] = np.random.randn(1000, 1000)

    print(f"✓ netCDF-4文件创建完成: {filepath}")


def create_hdf5_test_file(filepath):
    """创建HDF5格式测试文件"""
    if not HDF5_AVAILABLE:
        return

    print(f"\n创建HDF5测试文件: {filepath}")

    with h5py.File(filepath, "w") as f:
        f.attrs["title"] = "HDF5 Test File"
        f.attrs["institution"] = "Test Organization"
        f.attrs["creation_date"] = datetime.now().isoformat()
        f.attrs["version"] = "1.0"

        grp_sensor = f.create_group("/sensor_data")
        grp_metadata = f.create_group("/metadata")
        grp_nested = f.create_group("/sensor_data/calibration")
        grp_compressed = f.create_group("/compressed_data")

        dset_float = grp_sensor.create_dataset(
            "temperature",
            (100, 50, 30),
            dtype="f4",
            compression="gzip",
            compression_opts=4,
            chunks=(10, 10, 10),
            fillvalue=-999.0,
        )
        dset_float.attrs["units"] = "K"
        dset_float.attrs["standard_name"] = "air_temperature"
        dset_float.attrs["scale_factor"] = 0.1
        dset_float.attrs["add_offset"] = 273.15
        dset_float[:] = 273.15 + 20 * np.random.randn(100, 50, 30)

        dset_int = grp_sensor.create_dataset(
            "counts",
            (100, 50),
            dtype="i4",
            compression="gzip",
            compression_opts=5,
            chunks=(20, 20),
        )
        dset_int.attrs["units"] = "counts"
        dset_int.attrs["long_name"] = "Detector Counts"
        dset_int[:] = np.random.randint(0, 1000, (100, 50))

        data_large = np.random.randn(1000, 1000)

        dset_gzip = grp_compressed.create_dataset(
            "gzip_compressed",
            data=data_large,
            compression="gzip",
            compression_opts=6,
            chunks=(100, 100),
        )
        dset_gzip.attrs["compression"] = "gzip level 6"

        dset_lzf = grp_compressed.create_dataset(
            "lzf_compressed", data=data_large, compression="lzf", chunks=(100, 100)
        )
        dset_lzf.attrs["compression"] = "lzf"

        dset_chunked = grp_sensor.create_dataset(
            "chunked_data", (500, 400), dtype="f8", chunks=(50, 50)
        )
        dset_chunked.attrs["description"] = "Chunked dataset"
        dset_chunked[:, :] = np.random.randn(500, 400)

        dset_extendable = grp_sensor.create_dataset(
            "extendable", (100, 100), dtype="f4", maxshape=(None, 100), chunks=(10, 10)
        )
        dset_extendable.attrs["description"] = "Extendable dataset"
        dset_extendable[:, :] = np.random.randn(100, 100)
        dset_extendable.resize((150, 100))
        dset_extendable[100:150, :] = np.random.randn(50, 100)

        compound_dtype = np.dtype(
            [("time", "f8"), ("value", "f4"), ("quality", "i2"), ("name", "S20")]
        )
        compound_data = np.zeros(50, dtype=compound_dtype)
        compound_data["time"] = np.arange(50) * 60
        compound_data["value"] = 100 + 10 * np.random.randn(50)
        compound_data["quality"] = np.random.randint(0, 3, 50)
        for i in range(50):
            compound_data["name"][i] = f"point_{i:03d}".encode("ascii")

        dset_compound = grp_sensor.create_dataset(
            "compound_data", data=compound_data, compression="gzip", compression_opts=4
        )
        dset_compound.attrs["description"] = "Compound dataset with mixed types"

        string_dtype = h5py.string_dtype()
        string_data = [f"String_{i:04d}" for i in range(30)]
        dset_string = grp_metadata.create_dataset(
            "string_list", data=string_data, dtype=string_dtype
        )
        dset_string.attrs["description"] = "Variable length strings"

        grp_metadata.attrs["description"] = "Metadata group"
        grp_metadata.attrs["version"] = "2.0"

        scalar_data = np.array(42.0)
        dset_scalar = grp_metadata.create_dataset("scalar_value", data=scalar_data)
        dset_scalar.attrs["meaning"] = "Magic number"

        attr_array = np.array([1, 2, 3, 4, 5])
        dset_float.attrs["test_array"] = attr_array
        dset_float.attrs["special_chars"] = "Test: @#$%^&*()_+"

        grp_metadata["link_to_temperature"] = h5py.SoftLink("/sensor_data/temperature")

        try:
            external_file = os.path.join(os.path.dirname(filepath), "external_data.h5")
            if not os.path.exists(external_file):
                with h5py.File(external_file, "w") as ext_f:
                    ext_f.create_dataset("external_data", data=np.arange(100))
            grp_metadata["external_link"] = h5py.ExternalLink(
                external_file, "/external_data"
            )
        except Exception as e:
            print(f"  警告: 外部链接创建失败: {e}")

        enum_dtype = h5py.enum_dtype({"clear": 0, "cloudy": 1, "rain": 2, "snow": 3})
        enum_data = np.random.randint(0, 4, 100).astype(enum_dtype)
        dset_enum = grp_sensor.create_dataset(
            "weather_type", data=enum_data, dtype=enum_dtype
        )
        dset_enum.attrs["description"] = "Weather type enumeration"

    print(f"✓ HDF5文件创建完成: {filepath}")


def create_hdf4_test_file(filepath):
    """创建HDF4格式测试文件

    pyhdf的SD对象不支持file.attrs[...]方式写全局属性，
    必须用file.attr(name).set(type, value)；SDS数据集属性
    用sds.setdatastrs(label, unit, fmt, coordsys) + sds.attr(name).set(...)。
    """
    if not HDF4_AVAILABLE:
        return

    print(f"\n创建HDF4测试文件: {filepath}")

    try:
        hdf_file = SD(filepath, SDC.WRITE | SDC.CREATE)

        hdf_file.attr("title").set(SDC.CHAR, "HDF4 Test File")
        hdf_file.attr("institution").set(SDC.CHAR, "Test Organization")
        hdf_file.attr("created").set(SDC.CHAR, datetime.now().isoformat())
        hdf_file.attr("version").set(SDC.CHAR, "4.0")

        dims = (10, 20, 30)
        data_float = np.random.randn(*dims) * 10 + 50
        sds_float = hdf_file.create("temperature", SDC.FLOAT32, dims)
        sds_float.setdatastrs("Air Temperature", "K", "", "")
        sds_float.attr("standard_name").set(SDC.CHAR, "air_temperature")
        sds_float.attr("scale_factor").set(SDC.FLOAT32, 0.01)
        sds_float.attr("add_offset").set(SDC.FLOAT32, 273.15)
        sds_float[:] = data_float.astype(np.float32)

        data_int = np.random.randint(0, 100, (15, 25))
        sds_int = hdf_file.create("counts", SDC.INT32, (15, 25))
        sds_int.setdatastrs("Detector Counts", "counts", "", "")
        sds_int[:] = data_int.astype(np.int32)

        data_byte = np.random.randint(0, 255, (20, 30), dtype=np.uint8)
        sds_byte = hdf_file.create("image_data", SDC.UINT8, (20, 30))
        sds_byte.setdatastrs("Image data", "", "", "")
        sds_byte.attr("valid_range").set(SDC.UINT8, [0, 255])
        sds_byte[:] = data_byte

        attr_data = np.array([1.0, 2.0, 3.0, 4.0, 5.0], dtype=np.float32)
        sds_float.attr("test_array").set(SDC.FLOAT32, attr_data.tolist())
        sds_float.attr("description").set(SDC.CHAR, "Temperature dataset with scale and offset")

        data_missing = np.random.randn(8, 12) * 5 + 100
        mask = np.random.random((8, 12)) < 0.1
        data_missing[mask] = -999.0
        sds_missing = hdf_file.create("pressure", SDC.FLOAT32, (8, 12))
        sds_missing.setdatastrs("Air Pressure", "hPa", "", "")
        sds_missing.attr("_FillValue").set(SDC.FLOAT32, [-999.0])
        sds_missing.attr("missing_value").set(SDC.FLOAT32, [-999.0])
        sds_missing[:] = data_missing.astype(np.float32)

        hdf_file.end()
        print(f"✓ HDF4文件创建完成: {filepath}")

    except Exception as e:
        print(f"✗ HDF4文件创建失败: {e}")


def create_grib_test_files(base_dir):
    """使用eccodes直接创建GRIB1和GRIB2格式测试文件

    cfgrib是只读后端，无法写GRIB；eccodes 2.47起不再提供codes_handle类，
    改用codes_grib_new_from_samples + codes_set + codes_set_values + codes_write。
    """
    print(f"\n创建GRIB测试文件...")

    try:
        import eccodes

        def write_grib(filename, edition):
            filepath = os.path.join(base_dir, filename)
            print(f"\n创建GRIB{edition}测试文件: {filepath}")

            try:
                sample = "regular_ll_sfc_grib1" if edition == 1 else "regular_ll_sfc_grib2"
                handle = eccodes.codes_grib_new_from_samples(sample)

                eccodes.codes_set(handle, "centre", "ecmf")
                eccodes.codes_set(handle, "paramId", 130)
                eccodes.codes_set(handle, "shortName", "t")

                eccodes.codes_set(handle, "Ni", 20)
                eccodes.codes_set(handle, "Nj", 15)
                eccodes.codes_set(handle, "latitudeOfFirstGridPointInDegrees", 45.0)
                eccodes.codes_set(handle, "longitudeOfFirstGridPointInDegrees", -10.0)
                eccodes.codes_set(handle, "latitudeOfLastGridPointInDegrees", 55.0)
                eccodes.codes_set(handle, "longitudeOfLastGridPointInDegrees", 10.0)
                eccodes.codes_set(handle, "iDirectionIncrementInDegrees", 1.0)
                eccodes.codes_set(handle, "jDirectionIncrementInDegrees", 1.0)

                eccodes.codes_set(handle, "dataDate", 20240101)
                eccodes.codes_set(handle, "dataTime", 1200)
                eccodes.codes_set(handle, "stepType", "instant")

                if edition == 2:
                    eccodes.codes_set(handle, "typeOfLevel", "surface")
                    eccodes.codes_set(handle, "level", 0)
                    eccodes.codes_set(handle, "packingType", "grid_simple")

                data = 273.15 + 20 * np.random.randn(15 * 20)
                data = np.clip(data, 233.15, 323.15)
                eccodes.codes_set_values(handle, data)

                with open(filepath, "wb") as f:
                    eccodes.codes_write(handle, f)
                eccodes.codes_release(handle)

                print(f"✓ GRIB{edition}文件创建完成: {filepath}")

            except Exception as e:
                print(f"✗ GRIB{edition}文件创建失败: {e}")

        write_grib("test_grib1.grb", 1)
        write_grib("test_grib2.grb2", 2)

    except ImportError:
        print("✗ GRIB文件创建失败: 需要安装eccodes")
        print("  请安装: pip install eccodes")
    except Exception as e:
        print(f"✗ GRIB文件创建失败: {e}")


def create_bufr_test_file(filepath):
    """创建BUFR格式测试文件（eccodes 2.47：codes_bufr_new_from_samples）"""
    print(f"\n创建BUFR测试文件: {filepath}")

    try:
        import eccodes

        handle = eccodes.codes_bufr_new_from_samples("BUFR4")

        if handle is None:
            print("✗ 无法创建BUFR句柄，可能eccodes模板不存在")
            print("  提示: 可以手动创建BUFR文件或使用其他工具")
            return

        eccodes.codes_set(handle, "bufrHeaderCentre", 98)
        eccodes.codes_set(handle, "dataCategory", 0)
        eccodes.codes_set(handle, "masterTablesVersionNumber", 24)
        eccodes.codes_set(handle, "typicalYear", 2024)
        eccodes.codes_set(handle, "typicalMonth", 1)
        eccodes.codes_set(handle, "typicalDay", 15)
        eccodes.codes_set(handle, "typicalHour", 12)
        eccodes.codes_set(handle, "typicalMinute", 0)

        with open(filepath, "wb") as f:
            eccodes.codes_write(handle, f)
        eccodes.codes_release(handle)

        print(f"✓ BUFR文件创建完成: {filepath}")

    except ImportError:
        print("✗ BUFR文件创建失败: 需要eccodes")
        print("  请安装: conda install -c conda-forge eccodes")
    except Exception as e:
        print(f"✗ BUFR文件创建失败: {e}")


def create_xarray_grib_fallback(base_dir):
    """使用xarray创建GRIB的备选方案（保存为netCDF，并生成GRIB描述文件）"""
    print(f"\n创建xarray GRIB测试数据...")

    try:
        import xarray as xr

        # 生成测试数据
        times = pd.date_range("2024-01-01", periods=10, freq="6H")
        lats = np.linspace(45, 55, 15)
        lons = np.linspace(-10, 10, 20)

        ds = xr.Dataset(
            {
                "temperature": (
                    ["time", "latitude", "longitude"],
                    273.15 + 20 * np.random.randn(10, 15, 20),
                ),
                "pressure": (
                    ["time", "latitude", "longitude"],
                    1000 + 50 * np.random.randn(10, 15, 20),
                ),
            },
            coords={
                "time": times,
                "latitude": lats,
                "longitude": lons,
            },
            attrs={
                "title": "GRIB-like Test Dataset",
                "institution": "Test Organization",
                "created": datetime.now().isoformat(),
                "Conventions": "CF-1.8",
            },
        )

        ds["temperature"].attrs = {
            "units": "K",
            "standard_name": "air_temperature",
            "long_name": "Air Temperature",
        }
        ds["pressure"].attrs = {
            "units": "hPa",
            "standard_name": "air_pressure",
            "long_name": "Air Pressure",
        }

        # 保存为netCDF（可以作为GRIB的替代）
        nc_file = os.path.join(base_dir, "test_grib_like.nc")
        ds.to_netcdf(nc_file)
        print(f"✓ GRIB-like netCDF文件创建完成: {nc_file}")
        print(f"  (可作为GRIB数据的替代测试文件)")

    except Exception as e:
        print(f"✗ GRIB备选文件创建失败: {e}")


def create_xarray_test_files(base_dir):
    """使用xarray创建额外的测试文件"""
    print(f"\n使用xarray创建测试文件...")

    times = pd.date_range("2024-01-01", periods=30, freq="D")
    lats = np.linspace(-90, 90, 10)
    lons = np.linspace(0, 360, 20, endpoint=False)

    ds = xr.Dataset(
        {
            "temperature": (
                ["time", "lat", "lon"],
                273.15 + 20 * np.random.randn(30, 10, 20),
            ),
            "pressure": (
                ["time", "lat", "lon"],
                1000 + 50 * np.random.randn(30, 10, 20),
            ),
            "humidity": (["time", "lat", "lon"], 50 + 30 * np.random.randn(30, 10, 20)),
            "wind_speed": (
                ["time", "lat", "lon"],
                np.abs(10 + 5 * np.random.randn(30, 10, 20)),
            ),
        },
        coords={
            "time": times,
            "lat": lats,
            "lon": lons,
        },
        attrs={
            "title": "XArray Test Dataset",
            "institution": "Test Organization",
            "created": datetime.now().isoformat(),
            "Conventions": "CF-1.8",
        },
    )

    ds["temperature"].attrs = {
        "units": "K",
        "standard_name": "air_temperature",
        "scale_factor": 0.1,
        "add_offset": 273.15,
    }
    ds["pressure"].attrs = {
        "units": "hPa",
        "standard_name": "air_pressure",
    }
    ds["humidity"].attrs = {
        "units": "percent",
        "standard_name": "relative_humidity",
        "valid_min": 0,
        "valid_max": 100,
    }

    xr_file = os.path.join(base_dir, "test_xarray_netcdf4.nc")
    ds.to_netcdf(xr_file, engine="netcdf4", format="NETCDF4")
    print(f"✓ xarray netCDF4文件创建完成: {xr_file}")

    xr_file3 = os.path.join(base_dir, "test_xarray_netcdf3.nc")
    ds.to_netcdf(xr_file3, engine="netcdf4", format="NETCDF3_CLASSIC")
    print(f"✓ xarray netCDF3文件创建完成: {xr_file3}")

    comp = dict(zlib=True, complevel=5, shuffle=True)
    encoding = {var: comp for var in ds.data_vars}
    xr_file_comp = os.path.join(base_dir, "test_xarray_compressed.nc")
    ds.to_netcdf(xr_file_comp, engine="netcdf4", format="NETCDF4", encoding=encoding)
    print(f"✓ xarray压缩netCDF4文件创建完成: {xr_file_comp}")


def create_custom_datatype_test(filepath):
    """创建包含自定义数据类型的测试文件"""
    print(f"\n创建自定义数据类型测试文件: {filepath}")

    with nc4.Dataset(filepath, "w", format="NETCDF4") as rootgrp:
        rootgrp.title = "Custom Data Type Test"

        rootgrp.createDimension("obs", 50)

        obs_time = rootgrp.createVariable("obs_time", "f8", ("obs",))
        obs_time.units = "seconds since 2024-01-01"
        obs_time[:] = np.arange(50) * 3600

        obs_value = rootgrp.createVariable("obs_value", "f4", ("obs",))
        obs_value.units = "m/s"
        obs_value.long_name = "Wind Speed"
        obs_value[:] = 10 + 5 * np.random.randn(50)

        qc_flag = rootgrp.createVariable("qc_flag", "i1", ("obs",))
        qc_flag.long_name = "Quality Control Flag"
        qc_flag.flag_values = "0,1,2,3"
        qc_flag.flag_meanings = "good suspect bad missing"
        qc_flag[:] = np.random.randint(0, 4, 50)

        rootgrp.createDimension("station_id_len", 10)
        station_id = rootgrp.createVariable(
            "station_id", "S1", ("obs", "station_id_len")
        )
        for i in range(50):
            sid = f"S{i:03d}".encode("ascii")
            station_id[i, : len(sid)] = np.frombuffer(sid, dtype="S1")

    print(f"✓ 自定义数据类型文件创建完成: {filepath}")


def main():
    """主函数"""
    import argparse

    parser = argparse.ArgumentParser(description="科学数据格式测试文件生成器")
    parser.add_argument(
        "--output-dir",
        "-o",
        default="./mock-datasets",
        help="输出目录路径 (默认: ./mock-datasets)",
    )
    parser.add_argument(
        "--skip-grib", action="store_true", help="跳过GRIB/BUFR文件生成"
    )
    parser.add_argument("--skip-hdf4", action="store_true", help="跳过HDF4文件生成")
    args = parser.parse_args()

    output_dir = args.output_dir
    os.makedirs(output_dir, exist_ok=True)

    print(f"科学数据格式测试文件生成器")
    print(f"输出目录: {os.path.abspath(output_dir)}")
    print("开始生成测试文件...\n")

    global pd
    try:
        import pandas as pd
    except ImportError:
        print("警告: pandas未安装，xarray测试将跳过")
        pd = None

    # 创建各种格式的测试文件 - 传入完整路径
    create_netcdf3_test_file(os.path.join(output_dir, "test_netcdf3.nc"))
    create_netcdf4_test_file(os.path.join(output_dir, "test_netcdf4.nc"))
    create_hdf5_test_file(os.path.join(output_dir, "test_hdf5.h5"))

    if not args.skip_hdf4:
        create_hdf4_test_file(os.path.join(output_dir, "test_hdf4.hdf"))
    else:
        print("\n跳过HDF4文件生成 (用户指定)")

    if not args.skip_grib and GRIB_AVAILABLE:
        create_grib_test_files(output_dir)
        create_bufr_test_file(os.path.join(output_dir, "test_bufr.bufr"))
    elif args.skip_grib:
        print("\n跳过GRIB/BUFR文件生成 (用户指定)")
    else:
        print("\n跳过GRIB/BUFR文件生成 (eccodes未安装)")

    if pd is not None:
        create_xarray_test_files(output_dir)
    else:
        print("\n跳过xarray测试文件 (需要pandas)")

    create_custom_datatype_test(os.path.join(output_dir, "test_custom_datatypes.nc"))

    print_summary()


def print_summary():
    """打印生成文件的总结"""
    print("\n" + "=" * 60)
    print("测试文件生成完成!")
    print("=" * 60)
    print("\n生成的文件:")
    files = [
        "test_netcdf3.nc - netCDF-3 (经典格式)",
        "test_netcdf4.nc - netCDF-4 (压缩, 分组, 分块)",
        "test_hdf5.h5 - HDF5 (压缩, 复合类型, 链接)",
        "test_hdf4.hdf - HDF4 (如果pyhdf可用)",
        "test_grib1.grb - GRIB1 (如果eccodes可用)",
        "test_grib2.grb2 - GRIB2 (如果eccodes可用)",
        "test_bufr.bufr - BUFR (如果eccodes可用)",
        "test_xarray_netcdf4.nc - xarray生成的netCDF4",
        "test_xarray_netcdf3.nc - xarray生成的netCDF3",
        "test_xarray_compressed.nc - xarray压缩netCDF4",
        "test_custom_datatypes.nc - 自定义数据类型示例",
    ]
    for f in files:
        print(f"  • {f}")

    print("\n功能测试覆盖:")
    print("  ✓ 多种数据类型 (float, int, byte, string)")
    print("  ✓ scale_factor 和 add_offset")
    print("  ✓ 压缩 (zlib, gzip, lzf)")
    print("  ✓ 分块存储 (chunking)")
    print("  ✓ 缺失值处理 (_FillValue, missing_value)")
    print("  ✓ 分组 (groups)")
    print("  ✓ 元数据 (全局和变量属性)")
    print("  ✓ 坐标变量和维度")
    print("  ✓ 边界变量 (bounds)")
    print("  ✓ 枚举类型 (enum)")
    print("  ✓ 复合数据类型")
    print("  ✓ 可变长度字符串")
    print("  ✓ 软链接和外部链接")
    print("  ✓ 可扩展数据集")
    print("  ✓ 多维属性")
    print("=" * 60)


if __name__ == "__main__":
    main()

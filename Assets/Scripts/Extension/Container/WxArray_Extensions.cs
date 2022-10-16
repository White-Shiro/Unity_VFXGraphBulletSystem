public static class WxArray_Extensions {
	public static bool ArrayExist<T>(this T[] input, T value) {

		if(value != null && input != null) {
			foreach (var item in input) {
				if (item != null) {
					if (item.Equals(value)) {
						return true;
					}
				}
			}
		}

		return false;
	}

	//Function Overload - output the target Index
	public static bool ArrayExist<T>(this T[] input, T value, out int valueIndex) {
		if (value != null && input != null) {
			for (int i = 0; i < input.Length; i++) {
				if (input[i] != null) {
					if (input[i].Equals(value)) {
						valueIndex = i;
						return true;
					}
				}
			}
		}

		valueIndex = 0;
		return false;
	}
}


import cv2
import numpy as np

#resoultion = (3587, 2111)
#start_point = (1540, 2109)
#angle = 124.3907145
angle = 126.86989764584402129685561255909
halfAngle = angle/2
#size = (512, 1400)
#x_iter_left = 8
#x_iter_right = 8
#y_iter = 1
#is_cutting_front = True

def create_local_to_pixel_matrix(cur_size):
    return np.array([
    [1, 0, 0],
    [0, -1, cur_size[1]/2]
], np.int64)

def create_world_matrix(position, cur_size):
    return np.array([
        [1, 0, position[0] - 0],
        [0, 1, position[1] - cur_size[1]],
    ], np.int64)


def get_standard(cur_size):
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)
    
    up_point = np.array([0, cur_size[1] / 2], np.int64)
    down_point = np.array([0, -cur_size[1] / 2], np.int64)
    
    up_pivot_point = up_point + dright_vector + dleft_vector
    down_pivot_point = down_point + uright_vector + uleft_vector
    
    return (down_pivot_point, up_pivot_point, down_point, up_point)

def get_each_vector(cur_size):
    tan_value = np.tan(np.deg2rad(180.0 - angle) / 2)
    height = tan_value * (cur_size[0] / 2)
    width = cur_size[0] / 2
    
    uleft_vector = np.array([-width, height], np.int64)
    uright_vector =np.array([width, height], np.int64)
    
    dleft_vector = np.array([-width, -height], np.int64)
    dright_vector =np.array([width, -height], np.int64)
    
    return (uleft_vector, uright_vector, dleft_vector, dright_vector)

def transform_mask_poly(poly, position, cur_size):
    m = create_world_matrix(position, cur_size)

    for i in range(0, 4):
        p = poly[i][0]
        p = np.dot(create_local_to_pixel_matrix(cur_size), np.array([p[0], p[1], 1]))
        p = np.dot(m, np.array([p[0], p[1], 1]))

        poly[i][0] = p

    return poly

def get_right_front_mask(position, cur_size):
    down_pivot_point, up_pivot_point, down_point, up_point = get_standard(cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)

    poly = np.array([
        down_point,
        down_point + uright_vector,
        up_pivot_point + uright_vector,
        up_pivot_point
        ], np.int32)
    
    poly = poly.reshape((-1, 1, 2))

    return (transform_mask_poly(poly, position, cur_size), False, False)

def get_left_front_mask(position, cur_size):
    down_pivot_point, up_pivot_point, down_point, up_point = get_standard(cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)

    poly = np.array([
        down_point,
        down_point + uleft_vector,
        up_pivot_point + uleft_vector,
        up_pivot_point
        ], np.int64)
    
    poly = poly.reshape((-1, 1, 2))

    return (transform_mask_poly(poly, position, cur_size), True, False)

def get_right_back_mask(position, cur_size):
    down_pivot_point, up_pivot_point, down_point, up_point = get_standard(cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)

    poly = np.array([
        down_point + uright_vector,
        up_pivot_point + uright_vector,
        up_point,
        down_pivot_point
        ], np.int64)
    
    poly = poly.reshape((-1, 1, 2))

    return (transform_mask_poly(poly, position, cur_size), False, True)

def get_left_back_mask(position, cur_size):
    down_pivot_point, up_pivot_point, down_point, up_point = get_standard(cur_size)
    uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)

    poly = np.array([
        down_point + uleft_vector,
        up_pivot_point + uleft_vector,
        up_point,
        down_pivot_point
        ], np.int64)
    poly = poly.reshape((-1, 1, 2))

    return (transform_mask_poly(poly, position, cur_size), True, True)

def image_slice(image, pts, cur_size, is_side, is_left):
    mask = np.zeros((*image.shape[:2], 4), dtype=np.uint8)
    cv2.fillPoly(mask, [pts], (255, 255, 255, 255))
    masked_image = cv2.bitwise_and(image, mask)
    gray_masked_image = cv2.cvtColor(masked_image, cv2.COLOR_BGRA2GRAY)
    x, y, w, h = cv2.boundingRect(gray_masked_image)
    cropped_image = masked_image[y:y+h, x:x+w]


    resized_image = cropped_image
    canvas = np.zeros((cur_size[1], cur_size[0], 4), dtype=np.uint8)

    offset_y = 0

    if(is_side):
        tan_value = np.tan(np.deg2rad(180.0 - angle) / 2)
        height = tan_value * (cur_size[0] / 2)
        offset_y = int(np.ceil(height))

    yy = canvas.shape[0] - resized_image.shape[0] - offset_y

    if yy < 0:
        yy = 0

    if(is_left):
        canvas[
            yy : yy + resized_image.shape[0],
            0:resized_image.shape[1]
            ] = resized_image
    else:
        half_canvas_size = np.ceil(canvas.shape[1]/2).astype(np.int64)

        if(resized_image.shape[1] > half_canvas_size):
            half_canvas_size = half_canvas_size - 1

        canvas[
            yy : yy + resized_image.shape[0],
            half_canvas_size: half_canvas_size + resized_image.shape[1]
            ] = resized_image
        
    return canvas



def cut_and_save(origin_image,cur_point, cur_size, mask_func, name):
    pts, is_left, is_side = mask_func(cur_point, cur_size)
    img = image_slice(origin_image, pts, cur_size, is_side, is_left)

    cv2.imwrite(name, img)

    return pts
    #try:
    #    cv2.imwrite(name, img)
    #except:
    #    print("error")

def get_mask_front(origin_image_name, cur_size, cur_pos, cur_left_iter, cur_right_iter, cur_y_iter):
    _, up_pivot_point, down_point, _ = get_standard(cur_size)
    to_up_vector = (up_pivot_point - down_point)
    to_up_vector[1] = to_up_vector[1] * -1  

    image = cv2.imread(origin_image_name, cv2.IMREAD_UNCHANGED)
    mask_all = []

    for cur_y in range(0, cur_y_iter):
        uleft_vector, uright_vector, _, _ = get_each_vector(cur_size)
        cur_pos_left = np.array([cur_pos[0], cur_pos[1]]) + to_up_vector * -cur_y
        cur_pos_right = np.array([cur_pos[0], cur_pos[1]]) + to_up_vector * -cur_y
        mask_left_poly = []
        mask_right_poly = []

        for _ in range(0, cur_left_iter):
            pts, _, _ = get_left_front_mask(cur_pos_left, cur_size)
            cur_pos_left = cur_pos_left + np.array([uleft_vector[0], -uleft_vector[1]])
            mask_left_poly.append(pts)

        for _ in range(0, cur_right_iter):
            pts, _, _ = get_right_front_mask(cur_pos_right, cur_size)
            cur_pos_right = cur_pos_right + np.array([uright_vector[0], -uright_vector[1]])
            mask_right_poly.append(pts)

        mask_all.append((mask_left_poly, mask_right_poly))

    return (image, mask_all, False)

def get_mask_back(origin_image_name, cur_size,  cur_pos, cur_left_iter, cur_right_iter, cur_y_iter):
    down_pivot_point, up_pivot_point, down_point, up_point = get_standard(cur_size)
    to_up_vector = (up_pivot_point - down_point)
    to_up_vector[1] = to_up_vector[1]

    image = cv2.imread(origin_image_name, cv2.IMREAD_UNCHANGED)
    mask_all = []

    for cur_y in range(0, cur_y_iter):
        uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)
        cur_pos_left = np.array([cur_pos[0], cur_pos[1]]) + to_up_vector * -cur_y
        cur_pos_right = np.array([cur_pos[0], cur_pos[1]]) + to_up_vector * -cur_y
        mask_left_poly = []
        mask_right_poly = []

        for _ in range(0, cur_left_iter):
            pts, _, _ = get_left_back_mask(cur_pos_left, cur_size)
            cur_pos_left = cur_pos_left + np.array([uleft_vector[0], uleft_vector[1]])
            mask_left_poly.append(pts)

        for _ in range(0, cur_right_iter):
            pts, _, _ = get_right_back_mask(cur_pos_right, cur_size)
            cur_pos_right = cur_pos_right + np.array([uright_vector[0], uright_vector[1]])
            mask_right_poly.append(pts)

        mask_all.append((mask_left_poly, mask_right_poly))

    return (image, mask_all, True)

def save_image_front(origin_image_name, cur_size, cur_pos, trait, cur_left_iter, cur_right_iter, cur_y_iter):
    down_pivot_point, up_pivot_point, down_point, up_point = get_standard(cur_size)
    to_up_vector = (up_pivot_point - down_point)
    to_up_vector[1] = to_up_vector[1] * -1  

    cv2.namedWindow('Preview', cv2.WINDOW_NORMAL)
    cv2.resizeWindow('Preview', 1920, 1080)
    image = cv2.imread(origin_image_name, cv2.IMREAD_UNCHANGED)

    for cur_y in range(0, cur_y_iter):
        uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)
        cur_pos_left = np.array([cur_pos[0], cur_pos[1]]) + to_up_vector * cur_y
        cur_pos_right = np.array([cur_pos[0], cur_pos[1]]) + to_up_vector * cur_y

        for cur_x in range(0, cur_left_iter):
            pts = cut_and_save(image, cur_pos_left, cur_size, get_left_front_mask, f'front_{trait}_left_x{cur_x+1}_y{cur_y+1}.png')
            cur_pos_left = cur_pos_left + np.array([uleft_vector[0], -uleft_vector[1]])
            cv2.polylines(image, [pts], True, (0, 255, 255), thickness=2)

        for cur_x in range(0, cur_right_iter):
            pts = cut_and_save(image, cur_pos_right, cur_size, get_right_front_mask, f'front_{trait}_right_x{cur_x+1}_y{cur_y+1}.png')
            cur_pos_right = cur_pos_right + np.array([uright_vector[0], -uright_vector[1]])
            cv2.polylines(image, [pts], True, (0, 255, 255), thickness=2)

    cv2.imshow('Preview', image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

def save_image_back(origin_image_name, cur_size,  cur_pos, trait, cur_left_iter, cur_right_iter, cur_y_iter):
    down_pivot_point, up_pivot_point, down_point, up_point = get_standard(cur_size)
    to_up_vector = (up_pivot_point - down_point)
    to_up_vector[1] = to_up_vector[1]
    
    cv2.namedWindow('Preview', cv2.WINDOW_NORMAL)
    cv2.resizeWindow('Preview', 1920, 1080)
    image = cv2.imread(origin_image_name, cv2.IMREAD_UNCHANGED)

    for cur_y in range(0, cur_y_iter):
        uleft_vector, uright_vector, dleft_vector, dright_vector = get_each_vector(cur_size)
        cur_pos_left = np.array([cur_pos[0], cur_pos[1]]) + to_up_vector * cur_y
        cur_pos_right = np.array([cur_pos[0], cur_pos[1]]) + to_up_vector * cur_y

        for cur_x in range(0, cur_left_iter):
            pts = cut_and_save(image, cur_pos_left, cur_size, get_left_back_mask, f'back_{trait}_left_x{cur_x+1}_y{cur_y+1}.png')
            cur_pos_left = cur_pos_left + np.array([uleft_vector[0], uleft_vector[1]])
            cv2.polylines(image, [pts], True, (0, 255, 255), thickness=2)

        for cur_x in range(0, cur_right_iter):
            pts = cut_and_save(image, cur_pos_right, cur_size, get_right_back_mask, f'back_{trait}_right_x{cur_x+1}_y{cur_y+1}.png')
            cur_pos_right = cur_pos_right + np.array([uright_vector[0], uright_vector[1]])
            cv2.polylines(image, [pts], True, (0, 255, 255), thickness=2)


def input_xy(string):
    while True:
        input_str = input(string)
        try:
            input_str = input_str.replace('(', '').replace(')', '').replace(',', ' ')
            x, y = map(int, input_str.split())
            return (x, y)
        except:
            print("input error: " + input_str)

def input_number(string):
    while True:
        input_str = input(string)
        try:
            x = int(input_str)
            return x
        except:
            print("input error: " + input_str)

def input_and_save(image_name, mask_all_func, input_message):
    global global_file_name

    while True:
        print(input_message)
        is_pass = input("pass?(Y/Any): ")
        if is_pass == "Y" or is_pass == "y":
            print("\n")
            return
        
        cur_size = input_xy("tile size(x, y): ")
        cur_pos = input_xy("start pixel(x, y): ")   
        x_iter_left = input_number("left iteration: ")
        x_iter_right = input_number("right iteration: ")   
        #y_iter = input_number("y iteration: ") 문제가 생겨서 1로 고정   
        y_iter = 1
        print("\n")

        image, mask_all, is_back = mask_all_func(image_name, cur_size, cur_pos, x_iter_left, x_iter_right, y_iter)

        for item in mask_all:
           for pts in item[0]:
            cv2.polylines(image, [pts], True, (0, 255, 255), thickness=2)
           for pts in item[1]:
            cv2.polylines(image, [pts], True, (0, 255, 255), thickness=2)

        cv2.namedWindow('Preview', cv2.WINDOW_NORMAL)
        cv2.resizeWindow('Preview', 1920, 1080)
        cv2.imshow('Preview', image)
        input_key = cv2.waitKey(0)
        
        cv2.destroyAllWindows()
            
        rimage, mask_all, is_back = mask_all_func(image_name, cur_size, cur_pos, x_iter_left, x_iter_right, y_iter)



        if(input_key == ord('y') or input_key == ord('Y')):
            cur_y = 0

            for item in mask_all:
                cur_x = 0
                for pts in item[0]:
                    img = image_slice(rimage, pts, cur_size, is_back, True)
                    cv2.imwrite(f"{global_file_name}_{input_message}_left_x{cur_x+1}_y{cur_y+1}.png", img)
                    cur_x = cur_x + 1
                cur_x = 0
                for pts in item[1]:
                    img = image_slice(rimage, pts, cur_size, is_back, False)
                    cv2.imwrite(f"{global_file_name}_{input_message}_right_x{cur_x+1}_y{cur_y+1}.png", img)
                    cur_x = cur_x + 1
                cur_y = cur_y + 1
            break



global_file_name = input("enter file name: ")


input_and_save("front.png", get_mask_front,"front")
input_and_save("front.png", get_mask_front,"front_col")
input_and_save("back.png", get_mask_back,"back")
input_and_save("back.png", get_mask_back, "back_col")

#save_image_front("1.png", size, start_point, "render")
#save_image_front("1.png", (512, 512), start_point, "col")
#save_image_back("2.png", (512, 1420), (1922, 1416), "render", 5,5 ,1)
#save_image_back("2.png", (512, 512), (1922, 1416), "col")
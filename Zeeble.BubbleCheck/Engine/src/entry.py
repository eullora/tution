"""

 OMRChecker

 Author: Udayraj Deshmukh
 Github: https://github.com/Udayraj123

"""
import os
from pathlib import Path
import json
import cv2
from src.defaults import CONFIG_DEFAULTS
from src.template import Template
from src.utils.parsing import get_concatenated_response

def entry_point(omr_file_path,template_path):    
    process_file(omr_file_path, template_path)

def process_file(omr_file,template_path):    
    tuning_config = CONFIG_DEFAULTS
    template_file_path = Path(template_path)

    template = Template(template_file_path,tuning_config)

    template_output_cols = getattr(template, 'output_columns', [])    
            
    file_path = omr_file
    file_id = str(os.path.basename(file_path))
    try:
        in_omr = cv2.imread(str(file_path), cv2.IMREAD_GRAYSCALE)                        
        if in_omr is None:
            in_omr_err_output = json.dumps({ 'Code':100, 'Message':"in_omr is None. Unale to read image using cv2.imread"})
            print(in_omr_err_output)
            return

        template.image_instance_ops.reset_all_save_img()
        template.image_instance_ops.append_save_img(1, in_omr)
        processed_omr = template.image_instance_ops.apply_preprocessors(file_path, in_omr, template)

        if processed_omr is None:
            process_omr_err_output = json.dumps({ 'Code':101, 'Message':"processed_omr is None. Unale to process image in apply processor"})
            print(process_omr_err_output)
            return

        response_dict, _, _, _ = template.image_instance_ops.read_omr_response(template, image=processed_omr, name=file_id,save_dir=None)

        omr_response = get_concatenated_response(response_dict, template)
        
        resp_array = [omr_response.get(k, template.global_empty_val) for k in template_output_cols]

        result_dict = dict(zip(template_output_cols, resp_array))                                        
        
        success_output = {
            'Code':0,
            'Message':"Ok",
            'RollNumber': result_dict['RollNumber'],
            'Sheet': [f'{k}-{v}' for k, v in result_dict.items() if k.startswith('q')]
        }
        json_output = json.dumps(success_output)
        #os.system('cls' if os.name == 'nt' else 'clear')
        print(json_output);

    except Exception as e:
        err_output = json.dumps({'Code':500, 'Message': "Unhandled exception while processing the image. " + str(omr_file)})
        print(err_output)
        

